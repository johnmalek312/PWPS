using System;
using System.Collections.Generic;
using System.Text;
using PixelWorldsServer2.Networking.Server;
using System.IO;
using Discord.Net;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Kernys.Bson;
using PixelWorldsServer2.DataManagement;
using System.Linq;
using static PixelWorldsServer2.World.WorldInterface;
using System.Threading;
using System.Runtime.CompilerServices;
using PixelWorldsServer2.ItemsData.Door;

namespace PixelWorldsServer2.World
{
    public class WorldSession
    {
        private PWServer pServer = null;
        private byte version = 0x1;
        private List<Player> players = new List<Player>();
        public Dictionary<int, Collectable> collectables = new Dictionary<int, Collectable>();
        public int colID = 0;
        public string WorldID = "0";
        public short SpawnPointX = 36, SpawnPointY = 24;
        public string WorldName = string.Empty;
        public WorldInterface.WeatherType WeatherType = WorldInterface.WeatherType.None;
        public WorldInterface.LayerBackgroundType BackGroundType;
        private WorldTile[,] tiles = null;
        public List<WorldItemBase> worldItems = new List<WorldItemBase>();
        public int itemIndex = 0;
        public int GetSizeX() => tiles.GetUpperBound(0) + 1;
        public int GetSizeY() => tiles.GetUpperBound(1) + 1;

        public LockWorldData lockWorldData;
        public List<Player> Players => players;
        public Dictionary<string, long> banList = new Dictionary<string, long>();
        public void AddPlayer(Player p)
        {
            if (HasPlayer(p) == -1)
                players.Add(p);

            p.world = this;

            Save();
            using (var client = new DiscordWebhookClient("https://discord.com/api/webhooks/1073725309079265331/8yNyTL_6aSr95dwmEuVHdY8cWnxUoQ_dOfQJOLwQTeIKDOfu9FzvG0d5aJRVUZ98f4-g"))
            {
                var embed = new EmbedBuilder
                {
                    Title = "📈 LTPS Logs | Efe & Erdem",
                    Description = $"Player Name: {p.Data.Name}\nWorld Name: {p.world.WorldName}\nServer Online Count: {pServer.GetPlayersIngameCount()}"
                };

                client.SendMessageAsync(text: "```The world is saved, here is the info:```", embeds: new[] { embed.Build() });
            }
        }

        public int HasPlayer(Player p)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (p.Data.UserID == players[i].Data.UserID)
                    return i;

            }

            return -1;
        }
        public int HasPlayer(string p)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (p == players[i].Data.UserID)
                    return i;

            }

            return -1;
        }
        public void RemovePlayer(Player p)
        {
            int idx = HasPlayer(p);

            if (idx >= 0)
                players.RemoveAt(idx);
            p.world = null;
        }

        public void RemoveCollectable(int colID, Player toIgnore = null)
        {
            collectables.Remove(colID);
            BSONObject bObj = new BSONObject("RC");
            bObj["CollectableID"] = colID;
            Broadcast(ref bObj, toIgnore);

        }


        public void Broadcast(ref BSONObject bObj, params Player[] ignored) // ignored player can be used to ignore packet being sent to player itself.
        {
            foreach (var p in players)
            {
                if (ignored.Contains(p))
                    continue;

                p.Send(ref bObj);
            }
        }

        public void Drop(int id, int amt, double posX, double posY, int type, int gem = -1)
        {
            int cId = ++colID;
            BSONObject cObj = new BSONObject("nCo");
            cObj["CollectableID"] = cId;
            cObj["BlockType"] = id;
            cObj["Amount"] = amt;
            cObj["InventoryType"] = type;

            Collectable c = new Collectable();
            c.amt = (short)amt;
            c.item = (short)id;
            c.posX = posX * Math.PI;
            c.posY = posY * Math.PI;
            c.gemType = (short)gem;
            c.type = (short)type;

            cObj["PosX"] = c.posX;
            cObj["PosY"] = c.posY;
            cObj["IsGem"] = c.gemType > -1;
            cObj["GemType"] = c.gemType < 0 ? 0 : c.gemType;

            collectables[cId] = c;

            Broadcast(ref cObj);
        }

        public WorldSession(PWServer pServer, string worldName = "")
        {
            if (worldName == "")
                return;

            // load from SQL and File, if it doesn't exist, then generate.
            // first retrieve worldID, name, metadata... if fail, then generate world.
            this.pServer = pServer;
            string path = $"maps/{worldName}.map";

            if (!File.Exists(path))
            {
#if DEBUG
                Util.Log("Generating new world with name: " + worldName);
#endif
                // generate world
                Generate(worldName);
                return;
            }

            Util.Log("Attempting to load world from DB...");
            Deserialize(File.ReadAllBytes(path));
            this.WorldName = worldName;
        }

        public void Generate(string name)
        {
            // first, add new entry to sql:
            // todo filter the name from bad shit b4 release...
            SpawnPointX = (short)(1 + new Random().Next(79));
            WorldName = name;

            SetupTerrain();
        }

        public void Save()
        {
            string path = $"maps/{WorldName}.map";

            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x4); // version

                for (int y = 0; y < GetSizeY(); y++)
                {
                    for (int x = 0; x < GetSizeX(); x++)
                    {
                        var tile = GetTile(x, y);

                        ms.Write(BitConverter.GetBytes(tile.fg.id));
                        ms.Write(BitConverter.GetBytes(tile.bg.id));
                        ms.Write(BitConverter.GetBytes(tile.water.id));
                        ms.Write(BitConverter.GetBytes(tile.wire.id));
                    }
                }

                ms.Write(BitConverter.GetBytes(collectables.Values.Count));
                for (int i = 0; i < collectables.Values.Count; i++)
                {
                    var col = collectables.ElementAt(i).Value;
                    ms.Write(BitConverter.GetBytes(col.item));
                    ms.Write(BitConverter.GetBytes(col.amt));
                    ms.Write(BitConverter.GetBytes(col.posX));
                    ms.Write(BitConverter.GetBytes(col.posY));
                    ms.Write(BitConverter.GetBytes(col.gemType));
                    ms.Write(BitConverter.GetBytes(col.type));
                }
                ms.Write(BitConverter.GetBytes((int)BackGroundType));
                ms.Write(BitConverter.GetBytes((int)WeatherType));
                if (worldItems.Count > 0)
                {
                    BSONObject dobj = new BSONObject();
                    foreach (WorldItemBase item in worldItems)
                    {

                        if (item.blockType == BlockType.LockWorld)
                        {
                            dobj["W " + lockWorldData.x + " " + lockWorldData.y] = lockWorldData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.Door)
                        {
                            DoorData doorData = (DoorData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.CastleDoor)
                        {
                            CastleDoorData scifidoorData = (CastleDoorData)item;
                            dobj["W " + scifidoorData.x + " " + scifidoorData.y] = scifidoorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.ScifiDoor)
                        {
                            ScifiDoorData scifidoorData = (ScifiDoorData)item;
                            dobj["W " + scifidoorData.x + " " + scifidoorData.y] = scifidoorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.BarnDoor)
                        {
                            BarnDoorData doorData = (BarnDoorData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.GlassDoor)
                        {
                            GlassDoorData doorData = (GlassDoorData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.GlassDoorTinted)
                        {
                            GlassDoorTintedData doorData = (GlassDoorTintedData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.DungeonDoor || item.blockType == BlockType.DungeonDoorWhite)
                        {
                            DungeonDoorData doorData = (DungeonDoorData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }/*
                        else
                        if (item.blockType == BlockType.DoorFactionDark)
                        {
                            DoorFactionDarkData doorData = (DoorFactionDarkData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }
                        else
                        if (item.blockType == BlockType.DoorFactionLight)
                        {
                            DoorFactionLightData doorData = (DoorFactionLightData)item;
                            dobj["W " + doorData.x + " " + doorData.y] = doorData.GetAsBSON();
                        }*/
                    }
                    if (dobj.Keys.Count > 0)
                    {
                        byte[] dump = SimpleBSON.Dump(dobj);
                        ms.Write(BitConverter.GetBytes(dump.Length));
                        ms.Write(dump);
                    }
                    else
                    {
                        ms.Write(BitConverter.GetBytes((int)0));
                    }
                }
                else
                {
                    ms.Write(BitConverter.GetBytes((int)0));
                }
                File.WriteAllBytes(path, ms.ToArray());
                SpinWait.SpinUntil(() => Util.IsFileReady(path));
            }
        }

        public void SetupTerrain()
        {
            Util.Log("Setting up world terrain...");
            // empty world for now
            tiles = new WorldTile[80, 60];

            for (int i = 0; i < tiles.GetLength(1); i++)
            {
                for (int j = 0; j < tiles.GetLength(0); j++)
                {
                    tiles[j, i] = new WorldTile();
                }
            }

            for (int y = 0; y < SpawnPointY; y++)
            {
                for (int x = 0; x < GetSizeX(); x++)
                {
                    tiles[x, y].fg.id = 1;
                    tiles[x, y].bg.id = 2;
                }
            }
            BackGroundType = LayerBackgroundType.ForestBackground;
            WeatherType = WeatherType.None;
        }

        public WorldTile GetTile(int x, int y)
        {
            if (x >= GetSizeX() || y >= GetSizeY() || x < 0 || y < 0)
                return null;

            return tiles[x, y];
        }

        public BSONObject Serialize()
        {
            BSONObject wObj = new BSONObject();

            int tileLen = tiles.Length;
            int allocLen = tileLen * 2;

            byte[] blockLayerData = new byte[allocLen];
            byte[] backgroundLayerData = new byte[allocLen];
            byte[] waterLayerData = new byte[allocLen];
            byte[] wiringLayerData = new byte[allocLen];

            int width = GetSizeX();
            int height = GetSizeY();

            Util.Log($"Serializing world '{WorldName}' with width: {width} and height: {height}.");

            int pos = 0;
            for (int i = 0; i < tiles.Length; ++i)
            {
                int x = i % width;
                int y = i / width;

                if (x == SpawnPointX && y == SpawnPointY)
                    tiles[x, y].fg.id = 110;

                if (tiles[x, y].fg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].fg.id), 0, blockLayerData, pos, 2);
                if (tiles[x, y].bg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].bg.id), 0, backgroundLayerData, pos, 2);
                if (tiles[x, y].water.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].water.id), 0, waterLayerData, pos, 2);
                if (tiles[x, y].wire.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].wire.id), 0, wiringLayerData, pos, 2);
                pos += 2;
            }

            wObj[MsgLabels.MessageID] = MsgLabels.Ident.GetWorld;
            wObj["World"] = WorldName;
            wObj["BlockLayer"] = blockLayerData;
            wObj["BackgroundLayer"] = backgroundLayerData;
            wObj["WaterLayer"] = waterLayerData;
            wObj["WiringLayer"] = wiringLayerData;

            BSONObject cObj = new BSONObject();
            cObj["Count"] = collectables.Values.Count;

            for (int i = 0; i < collectables.Values.Count; i++)
            {
                var col = collectables.ElementAt(i).Value.GetAsBSON();
                var kv = collectables.ElementAt(i);

                col["CollectableID"] = kv.Key;
                cObj[$"C{i}"] = col;
            }
            BSONObject dobj = new BSONObject();
            foreach (WorldItemBase item in worldItems)
            {

                if (item.blockType == BlockType.LockWorld)
                {
                    var a = lockWorldData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.Door)
                {
                    DoorData doorData = (DoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.ScifiDoor)
                {
                    ScifiDoorData doorData = (ScifiDoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.BarnDoor)
                {
                    BarnDoorData doorData = (BarnDoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.GlassDoor)
                {
                    GlassDoorData doorData = (GlassDoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.CastleDoor)
                {
                    CastleDoorData doorData = (CastleDoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.GlassDoorTinted)
                {
                    GlassDoorTintedData doorData = (GlassDoorTintedData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.DungeonDoor || item.blockType == BlockType.DungeonDoorWhite)
                {
                    DungeonDoorData doorData = (DungeonDoorData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }/*
                else
                if (item.blockType == BlockType.DoorFactionDark)
                {
                    DoorFactionDarkData doorData = (DoorFactionDarkData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }
                else
                if (item.blockType == BlockType.DoorFactionLight)
                {
                    DoorFactionLightData doorData = (DoorFactionLightData)item;
                    var a = doorData.GetAsBSON();
                    dobj["W " + a["posX"].int32Value + " " + a["posY"].int32Value] = a;
                }*/
            }


            List<int>[] layerHits = new List<int>[4];
            for (int j = 0; j < layerHits.Length; j++)
            {
                layerHits[j] = new List<int>();
                layerHits[j].AddRange(Enumerable.Repeat(0, tileLen));
            }

            List<int>[] layerHitBuffers = new List<int>[4];
            for (int j = 0; j < layerHitBuffers.Length; j++)
            {
                layerHitBuffers[j] = new List<int>();
                layerHitBuffers[j].AddRange(Enumerable.Repeat(0, tileLen));
            }

            wObj["BlockLayerHits"] = layerHits[0];
            wObj["BackgroundLayerHits"] = layerHits[1];
            wObj["WaterLayerHits"] = layerHits[2];
            wObj["WiringLayerHits"] = layerHits[3];

            wObj["BlockLayerHitBuffers"] = layerHitBuffers[0];
            wObj["BackgroundLayerHitBuffers"] = layerHitBuffers[1];
            wObj["WaterLayerHitBuffers"] = layerHitBuffers[2];
            wObj["WiringLayerHits"] = layerHitBuffers[3];

            // change to template null count for optimization soon...
            BSONObject wLayoutType = new BSONObject();
            wLayoutType["Count"] = 0;
            BSONObject wBackgroundType = new BSONObject();
            wBackgroundType["Count"] = (int)BackGroundType;
            BSONObject wMusicSettings = new BSONObject();
            wMusicSettings["Count"] = 0;

            BSONObject wStartPoint = new BSONObject();
            wStartPoint["x"] = (int)SpawnPointX; wStartPoint["y"] = (int)SpawnPointY;

            BSONObject wSizeSettings = new BSONObject();
            wSizeSettings["WorldSizeX"] = width; wSizeSettings["WorldSizeY"] = height;
            BSONObject wGravityMode = new BSONObject();
            wGravityMode["Count"] = 0;
            BSONObject wRatings = new BSONObject();
            wRatings["Count"] = 0;
            BSONObject wRaceScores = new BSONObject();
            wRaceScores["Count"] = 0;
            BSONObject wLightingType = new BSONObject();
            wLightingType["Count"] = 0;
            BSONObject wWeatherType = new BSONObject();
            wWeatherType["Count"] = (int)WeatherType;


            wObj["WorldLayoutType"] = wLayoutType;
            wObj["WorldBackgroundType"] = wBackgroundType;
            wObj["WorldMusicIndex"] = wMusicSettings;
            wObj["WorldStartPoint"] = wStartPoint;
            wObj["WorldItemId"] = 0;
            wObj["WorldSizeSettings"] = wSizeSettings;
            //wObj["WorldGravityMode"] = wGravityMode;
            wObj["WorldRatingsKey"] = wRatings;
            wObj["WorldItemId"] = 1;
            wObj["InventoryId"] = 1;
            wObj["RatingBoardCountKey"] = 0;
            wObj["QuestStarterItemSummerCountKey"] = 0;
            wObj["WorldRaceScoresKey"] = wRaceScores;
            wObj["WorldTagKey"] = 0;
            wObj["PlayerMaxDeathsCountKey"] = 0;
            wObj["RatingBoardDateTimeKey"] = DateTimeOffset.UtcNow.Date;
            wObj["WorldLightingType"] = wLightingType;
            wObj["WorldWeatherType"] = wWeatherType;

            BSONObject pObj = new BSONObject();

            wObj["PlantedSeeds"] = pObj;
            wObj["Collectables"] = cObj;
            wObj["WorldItems"] = dobj;
            return wObj;
        }

        public void Deserialize(byte[] binary)
        {
            // load binary from file
            tiles = new WorldTile[80, 60]; // only this dimension is supported anyways
            for (int i = 0; i < tiles.GetLength(1); i++)
            {
                for (int j = 0; j < tiles.GetLength(0); j++)
                {
                    tiles[j, i] = new WorldTile();
                }
            }

            version = binary[0];

            int pos = 1;
            for (int y = 0; y < GetSizeY(); y++)
            {
                for (int x = 0; x < GetSizeX(); x++)
                {
                    var tile = tiles[x, y];

                    tile.fg.id = BitConverter.ToInt16(binary, pos);
                    tile.bg.id = BitConverter.ToInt16(binary, pos + 2);
                    tile.water.id = BitConverter.ToInt16(binary, pos + 4);
                    tile.wire.id = BitConverter.ToInt16(binary, pos + 6);

                    if (tile.fg.id == 110)
                    {
                        SpawnPointX = (short)x;
                        SpawnPointY = (short)y;
                    }

                    pos += 8;
                }
            }

            int dropCount = BitConverter.ToInt32(binary, pos); pos += 4;
            for (int i = 0; i < dropCount; i++)
            {
                Collectable c = new Collectable();
                c.item = BitConverter.ToInt16(binary, pos);
                c.amt = BitConverter.ToInt16(binary, pos + 2);
                c.posX = BitConverter.ToDouble(binary, pos + 4);
                c.posY = BitConverter.ToDouble(binary, pos + 12);
                c.gemType = BitConverter.ToInt16(binary, pos + 20);
                    c.type = BitConverter.ToInt16(binary, pos + 22);
                pos += 24;
                collectables[++colID] = c;
            }
            if (pos < binary.Length)
            {
                BackGroundType = (WorldInterface.LayerBackgroundType)BitConverter.ToInt32(binary, pos);
                WeatherType = (WorldInterface.WeatherType)BitConverter.ToInt32(binary, pos + 4);
                pos += 8;

            }
            int len = BitConverter.ToInt32(binary, pos);
            
            pos += 4;
            if (len > 0)
            {
                BSONObject dobj = SimpleBSON.Load(new ArraySegment<byte>(binary, pos, len).ToArray());
                foreach (string key in dobj.Keys)
                {
                    if (dobj[key][WorldItemBase.classKey].stringValue == "LockWorldData")
                    {
                        lockWorldData = new LockWorldData(dobj[key]["itemId"].int32Value);
                        lockWorldData.SetViaBSON(dobj[key] as BSONObject);
                        if(!worldItems.Contains(lockWorldData))worldItems.Add(lockWorldData);
                    }else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "DoorData")
                    {
                        DoorData doorData = new DoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "ScifiDoorData")
                    {
                        ScifiDoorData doorData = new ScifiDoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "CastleDoorData")
                    {
                        CastleDoorData doorData = new CastleDoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "BarnDoorData")
                    {
                        BarnDoorData doorData = new BarnDoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "GlassDoorData")
                    {
                        GlassDoorData doorData = new GlassDoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "GlassDoorTintedData")
                    {
                        GlassDoorTintedData doorData = new GlassDoorTintedData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "DungeonDoorData")
                    {
                        DungeonDoorData doorData = new DungeonDoorData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }
                    /*else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "DoorFactionLightData")
                    {
                        DoorFactionLightData doorData = new DoorFactionLightData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }else
                    if (dobj[key][WorldItemBase.classKey].stringValue == "DoorFactionDarkData")
                    {
                        DoorFactionDarkData doorData = new DoorFactionDarkData(dobj[key]["itemId"].int32Value);
                        doorData.SetViaBSON(dobj[key] as BSONObject);
                        worldItems.Add(doorData);
                    }*/
                }
            }   

        }
        public bool CanKick(Player p, string id)
        {
            WorldSession w = p.world;
            if (w == null) return false;
            if (p == null) return false;
            if (w.lockWorldData == null) return false;
            if (p.world.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID))
                return true;

            return false;
        }
        public bool CanBan(Player p, string id)
        {
            WorldSession w = p.world;
            if (w == null) return false;
            if (p == null) return false;
            if (w.lockWorldData == null) return false;
            if (p.world.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID))
                return true;

            return false;
        }
        public bool CanSummon(Player p, string id)
        {
            WorldSession w = p.world;

            if (w == null) return false;
            if (p == null) return false;
            if (w.lockWorldData == null) return false;
            if (p.world.lockWorldData.DoesPlayerHaveMinorAccessToLock(p.Data.UserID))
                return true;

            return false;
        }

        public bool SetBlock(int x, int y, short blockType, Player p)
        {
            if (this.GetTile(x, y).fg.id != 0) return false;
            if (blockType == (short)BlockType.LockWorld)
            {
                foreach (WorldItemBase item in worldItems)
                {
                    if (item.blockType == BlockType.LockWorld)
                    {
                        return false;
                    }
                }
                this.itemIndex++;
                lockWorldData = new LockWorldData(itemIndex);
                lockWorldData.SetPlayerWhoOwnsLockId(p.Data.UserID);
                lockWorldData.SetPlayerWhoOwnsLockName(p.Data.Name);
                lockWorldData.SetLastActivatedTime(DateTime.UtcNow);
                lockWorldData.SetCreationTime(DateTime.UtcNow);
                lockWorldData.x = x; lockWorldData.y = y;
                worldItems.Add(lockWorldData);
                var t = this.GetTile(x, y);
                t.fg.id = blockType;
                t.fg.damage = 0;
                t.fg.lastHit = 0;
                return true;
            }

            Item it = ItemDB.GetByID((int)blockType);
            switch (it.type)
            {
                case 3:
                    {
                        var t = this.GetTile(x, y);
                        t.water.id = blockType;
                        t.water.damage = 0;
                        t.water.lastHit = 0;
                        break;
                    }
                default:
                    {

                        var t = this.GetTile(x, y);
                        t.fg.id = blockType;
                        t.fg.damage = 0;
                        t.fg.lastHit = 0;

                        break;
                    }
            }
            return true;
        }
        public WorldItemBase FindItemBaseWithID(int id)
        {
            WorldItemBase worldItem = worldItems.Find(item => item.itemId == id);
            return worldItem;
        }

        public bool IsPlayerBanned(Player p)
        {
            if(banList.ContainsKey(p.Data.UserID))
            {
                if (banList[p.Data.UserID] > DateTime.UtcNow.Ticks)
                {
                    return true;
                }
            }
            banList.Remove(p.Data.UserID);
            return false;
        }

        ~WorldSession()
        {

        }
    }
}

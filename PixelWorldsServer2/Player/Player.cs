using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using FeatherNet;
using Kernys.Bson;
using System.Threading.Tasks;
using System.Timers;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.Networking.Server;
using PixelWorldsServer2.World;
using static PixelWorldsServer2.World.WorldInterface;
using System.Linq;
using System.Collections;

namespace PixelWorldsServer2
{
    public class Player
    {
        private PWServer pServer = null;
        public PlayerSettings pSettings = null;
        public bool isInGame = false; // when the player has logon and is inside.
        public bool sendPing = false;
        public bool isLoadingWorld = false;
        public PlayerInventoryManager inventoryManager;
        public bool IsOnline() => isInGame && Client != null;
        public struct PlayerData
        {
            public Player player;
            public string UserID;
            public int Gems, Coins;
            public string CognitoID, Token;
            public string Name;
            public string RealName;
            public string LastIP;
            public Dictionary<int, short> Inventory;
            public double PosX, PosY;
            public int Anim, Dir;
            public AdminStatus adminStatus;
            public BSONObject BSON;
            public RecentWorlds recentWorlds;
            public bool adminWantsToGoThroughDoors;
            public bool modWantsToGoThroughDoors;
            public bool adminWantsToEditWorld;
            public bool adminWantsToBeSummoned;
            public bool adminWantsToGoGhostMode;
            public bool adminWantsToGoUndercoverMode;
        }

        public enum AdminStatus
        {
            AdminStatus_None,
            AdminStatus_Moderator,
            AdminStatus_Admin,
            AdminStatus_Influencer
        }

        private PlayerData pData; // basically acts like a save, this is not the data that is assigned to the FeatherNet session itself.
        private FeatherClient fClient = null;
        private List<BSONObject> packets = new List<BSONObject>();
        public World.WorldSession world = null;
        public string GetWorldName()
        {
            if (world == null)
                return "[WORLD MENU]";

            return world == null ? "<World Menu>" : world.WorldName;
        }
        public Player(FeatherClient fClient = null)
        {
            if (fClient != null)
            {
                this.fClient = fClient;
                pServer = fClient.link as PWServer;
                this.pSettings = new PlayerSettings();

                pData.player = this;
                pData.UserID = "";
                pData.Gems = 0;
                pData.Coins = 0;
                pData.CognitoID = "";
                pData.Token = "";
                pData.Name = "";
                pData.LastIP = "0.0.0.0";
                pData.Inventory = new Dictionary<int, short>();
                pData.BSON = new BSONObject();
                fClient.data = pData; // interlink
                pData.recentWorlds = new RecentWorlds();
            }
        }
        public Player(SQLiteDataReader reader)
        {
            pData.player = this;
            this.pSettings = new PlayerSettings((int)reader["Settings"]);
            pData.UserID = (string)reader["ID"];
            pData.Gems = (int)reader["Gems"];
            pData.Coins = (int)reader["ByteCoins"];
            pData.Name = (string)reader["Name"];
            pData.LastIP = (string)reader["IP"];
            pData.adminStatus = (Player.AdminStatus)Convert.ToInt32(reader["AdminStatus"]);
            object inven = reader["Inventory"];
            pData.recentWorlds = new RecentWorlds();
            this.SetRecentWorlds((string)reader["RecentWorlds"]);
            byte[] invData = null;

            if (!Convert.IsDBNull(inven))
                invData = (byte[])inven;

            object bsonObj = reader["Inventory"];
            byte[] bsonData = null;

            if (!Convert.IsDBNull(bsonObj))
                bsonData = (byte[])bsonObj;

            if (bsonData != null)
            {
                try
                {
                    pData.BSON = SimpleBSON.Load(bsonData);
                }
                catch
                {
                    pData.BSON = new BSONObject();
                }
            }
            else
            {
                pData.BSON = new BSONObject();
            }
            pData.Inventory = new Dictionary<int, short>();
            inventoryManager = new PlayerInventoryManager(this);
            this.inventoryManager.InitInventoryFromBinary((byte[])inven);
        }

        public FeatherClient Client { get { return fClient; } }
        public ref dynamic ClientData { get { return ref fClient.data; } }

        public ref PlayerData Data => ref pData;

        public void Tick()
        {
            if (Client != null)
            {
                while (packets.Count > 0)
                {
                    Client.Send(packets[0]);
                    packets.RemoveAt(0);
                }
            }
        }

        public void SendPing()
        {
            foreach (var pac in packets)
            {
                if (pac["ID"] == "p")
                    return;
            }

            Send(ref MsgLabels.pingBson);
            Save();
        }

        public void SetClient(FeatherClient fClient)
        {
            this.fClient = fClient;
            if (fClient != null)
            {
                if (fClient.link != null)
                    pServer = fClient.link as PWServer;
            }
        }

        public void SelfChat(string txt)
        {
            BSONObject c = new BSONObject("WCM");
            c[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FF0000>System",
                world.WorldName,
                world.WorldName,
                1,
                txt);

            Send(ref c);
        }

        public void Send(ref BSONObject packet) => packets.Add(packet);
        public void RemoveGems(int amt)
        {
            Data.Gems -= amt;
            BSONObject bObj = new BSONObject("RG");
            bObj["Amt"] = amt;

            Send(ref bObj);
        }

        public void AddGems(int amt)
        {
            Data.Gems += amt;
            BSONObject bObj = new BSONObject("GG");
            bObj["Amt"] = amt;

            Send(ref bObj);
        }

        public bool IsUnregistered()
        {
            return pData.Name.StartsWith("LTPS_");
        }

        public List<string>[] GetRecentWorlds()
        {
            List<string>[] a = new List<string>[2];
            a[0]  = this.Data.recentWorlds.WorldNames.ToList();
            a[1]  = this.Data.recentWorlds.WorldIds.ToList();
            return a;
        }

        public string GetRecentWorldsAsString()
        {
            List<string>[] a = new List<string>[2];
            a[0] = this.Data.recentWorlds.WorldNames.ToList();
            a[1] = this.Data.recentWorlds.WorldIds.ToList();
            if (a[0].Count == 0) return "";
            return (string.Join(",", a[0]) + ";" + string.Join(",", a[1]));
        }

        public void SaveRecentWorlds()
        {
            var sql = pServer.GetSQL();
            var cmd = sql.Make("UPDATE players SET " +
                "RecentWorlds=@RecentWorlds " + 
                "WHERE ID=@ID");

            cmd.Parameters.AddWithValue("@RecentWorlds", GetRecentWorldsAsString());

            cmd.Parameters.AddWithValue("@ID", Data.UserID);

            if (sql.PreparedQuery(cmd) > 0)
            {
                //Util.Log($"Player ID: {Data.UserID} ('{Data.Name}') saved.");
            }
        }

        public void SetRecentWorlds(string data) 
        {
            if (data == null || data == "")
            {
                this.pData.recentWorlds.WorldIds.Clear();
                this.pData.recentWorlds.WorldNames.Clear();
            }
            else
            {
                string[] worldsplit = data.Split(';');
                string[] worldNames = worldsplit[0].Split(",");
                string[] worldIds = worldsplit[1].Split(",");
                pData.recentWorlds.WorldNames = new Stack<string>(worldNames);
                pData.recentWorlds.WorldIds = new Stack<string>(worldIds);
            }
            
        }
        public void AddRecentWorld(string worldName, string worldId)
        {
            if(this.Data.recentWorlds.WorldNames.Count() < Config.maxRecentWorlds)
            {
                if(this.Data.recentWorlds.WorldNames.Contains(worldName))
                {
                    Stack<string> tempStack = new Stack<string>();
                    Stack<string> tempStack2 = new Stack<string>();

                    while (Data.recentWorlds.WorldNames.Count > 0)
                    {
                        string currentElement = Data.recentWorlds.WorldNames.Pop();
                        string currentElement2 = Data.recentWorlds.WorldIds.Pop();

                        if (currentElement != worldName)
                        {
                            tempStack.Push(currentElement);
                            tempStack2.Push(currentElement2);
                        }
                    }
                    while (tempStack.Count > 0)
                    {
                        Data.recentWorlds.WorldNames.Push(tempStack.Pop());
                        Data.recentWorlds.WorldIds.Push(tempStack2.Pop());
                    }
                }
                this.Data.recentWorlds.WorldNames.Push(worldName);
                this.Data.recentWorlds.WorldIds.Push(worldId);
            }
            else
            {
                if (this.Data.recentWorlds.WorldNames.Contains(worldName))
                {
                    Stack<string> tempStack = new Stack<string>();
                    Stack<string> tempStack2 = new Stack<string>();

                    while (Data.recentWorlds.WorldNames.Count > 0)
                    {
                        string currentElement = Data.recentWorlds.WorldNames.Pop();
                        string currentElement2 = Data.recentWorlds.WorldIds.Pop();

                        if (currentElement != worldName)
                        {
                            tempStack.Push(currentElement);
                            tempStack2.Push(currentElement2);
                        }
                    }
                    while (tempStack.Count > 0)
                    {
                        Data.recentWorlds.WorldNames.Push(tempStack.Pop());
                        Data.recentWorlds.WorldIds.Push(tempStack2.Pop());
                    }
                }
                this.Data.recentWorlds.WorldNames.Pop();
                this.Data.recentWorlds.WorldIds.Pop();
                this.Data.recentWorlds.WorldNames.Push(worldName);
                this.Data.recentWorlds.WorldIds.Push(worldId);
            }
        }
        public class RecentWorlds
        {
            public Stack<string> WorldNames = new Stack<string>();
            public Stack<string> WorldIds = new Stack<string>();
        }


        public void Save()
        {

            //Util.Log("Saving player...");

            var sql = pServer.GetSQL();
            var cmd = sql.Make("UPDATE players SET " +
                "Gems=@Gems, " +
                "ByteCoins=@ByteCoins, " +
                "IP=@IP, " +
                "Inventory=@Inventory, " +
                "Settings=@Settings, " +
                "BSON=@BSON " +
                "WHERE ID=@ID");

            cmd.Parameters.AddWithValue("@Gems", Data.Gems);
            cmd.Parameters.AddWithValue("@ByteCoins", Data.Coins);
            cmd.Parameters.AddWithValue("@IP", Data.LastIP);
            cmd.Parameters.AddWithValue("@Settings", pSettings.GetSettings());
            if (Data.BSON == null)
                Data.BSON = new BSONObject();

            cmd.Parameters.AddWithValue("@BSON", SimpleBSON.Dump(Data.BSON));

            byte[] invData = this.inventoryManager.GetInventoryAsBinary();
            cmd.Parameters.Add("@Inventory", DbType.Binary);
            cmd.Parameters["@Inventory"].Value = invData;

            cmd.Parameters.AddWithValue("@ID", Data.UserID);

            if (sql.PreparedQuery(cmd) > 0)
            {
                //Util.Log($"Player ID: {Data.UserID} ('{Data.Name}') saved.");
            }
        }
        public void SendRemoveItemInventory(BlockType blockType, InventoryItemType inventoryType, int am)
        {
            BSONObject rObj = new BSONObject();
            rObj["ID"] = MsgLabels.Ident.RemoveInventoryItem;
            rObj["rI"] = new BSONObject();
            rObj["rI"]["CollectableID"] = 0;
            rObj["rI"]["BlockType"] = (int)blockType;
            rObj["rI"]["Amount"] = am;
            rObj["rI"]["InventoryType"] = (int)inventoryType;
            rObj["rI"]["PosX"] = (Double)0;
            rObj["rI"]["PosY"] = (Double)0;
            rObj["rI"]["IsGem"] = false;
            rObj["rI"]["GemType"] = 0;
            this.Client.Send(rObj);
        }
    }
}

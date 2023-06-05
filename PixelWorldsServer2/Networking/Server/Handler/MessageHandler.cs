using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Discord.WebSocket;
using Discord.Net;
using Discord;
using Discord.Webhook;
using static PixelWorldsServer2.World.WorldSession;
using static PixelWorldsServer2.World.WorldInterface;
using static PixelWorldsServer2.Player;
using static FeatherNet.FeatherEvent;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace PixelWorldsServer2.Networking.Server
{
    public class MessageHandler
    {
        private PWServer pServer = null;

        public MessageHandler(PWServer pwServer)
        {
            pServer = pwServer;
        }
        private List<WorldSession> worlds = new List<WorldSession>();
        public List<WorldSession> GetWorlds() => worlds;

        private List<InventoryKey> itemList = new List<InventoryKey>();
        public List<InventoryKey> Items => itemList;

        private PlayerData pData;


        public void ProcessBSONPacket(FeatherClient client, BSONObject bObj)
        {
            if (pServer == null)
            {
                Util.Log("ERROR cannot process BSON packet when pServer is null!");
                return;
            }

            if (!bObj.ContainsKey("mc"))
            {
                Util.Log("Invalid bson packet (no mc!)");
                client.DisconnectLater();
                return; // Invalid Pixel Worlds BSON packet/!
            }

#if RELEASE
#endif
            int messageCount = bObj["mc"];


            Player p = client.data == null ? null : ((Player.PlayerData)client.data).player;
            for (int i = 0; i < messageCount; i++)
            {
                if (!bObj.ContainsKey($"m{i}"))
                    throw new Exception($"Non existing message object failed to be accessed by index '{i}'!");

                BSONObject mObj = bObj[$"m{i}"] as BSONObject;
                string mID = mObj[MsgLabels.MessageID];
                if (mObj["ID"].stringValue != "mP" && mObj["ID"].stringValue != "ST")
                    ReadBSON(mObj, Log: Util.LogClient);
                try
                {
                    switch (mID)
                    {

                        case MsgLabels.Ident.VersionCheck:
                            Util.Log("Client requests version check, responding now...");
                            //#endif
                            BSONObject resp = new BSONObject();
                            resp[MsgLabels.MessageID] = MsgLabels.Ident.VersionCheck;
                            resp[MsgLabels.VersionNumberKey] = pServer.Version;
                            client.Send(resp);
                            break;

                        case MsgLabels.Ident.GetPlayerData:
                            HandlePlayerLogon(client, mObj);
                            break;

                        case MsgLabels.Ident.TryToJoinWorld:
                            HandleTryToJoinWorld(p, mObj);
                            break;

                        case "TTJWR":
                            HandleTryToJoinWorldRandom(p);
                            break;

                        case MsgLabels.Ident.GetWorld:
                            HandleGetWorld(p, mObj);
                            break;

                        case "GSb":
                            if (p != null)
                                p.isLoadingWorld = false;

                            p.Send(ref mObj);
                            break;

                        case "WCM":
                            HandleWorldChatMessage(p, mObj);
                            break;

                        case "MWli":
                            HandleMoreWorldInfo(p, mObj);
                            break;

                        case "PSicU":
                            HandlePlayerStatusChange(p, mObj);
                            break;

                        case "BIPack":
                            HandleShopPurchase(p, mObj);
                            break;

                        case "RenamePlayer":
                            HandleRenamePlayer(p, mObj);
                            break;

                        case "rOP": // request other players
                            HandleSpawnPlayer(p, mObj);
                            HandleRequestOtherPlayers(p, mObj);
                            break;

                        case "GM":
                            HandleGlobalMessage(p, mObj);
                            break;

                        case "RtP":
                            break;

                        case MsgLabels.Ident.LeaveWorld:
                            HandleLeaveWorld(p, mObj);
                            break;

                        case "rAI": // request AI (bots, etc.)??
                            HandleRequestAI(p, mObj);
                            break;

                        case "rAIp": // ??
                            HandleRequestAIp(p, mObj);
                            break;

                        case "Rez":
                            if (p == null)
                                break;

                            if (p.world == null)
                                break;

                            mObj["U"] = p.Data.UserID;
                            p.world.Broadcast(ref mObj, p);
                            break;

                        case MsgLabels.Ident.WearableUsed:
                            HandleWearableUsed(p, mObj);
                            break;
                        case MsgLabels.Ident.WearableRemoved:
                            HandleWearableRemoved(p, mObj);
                            break;

                        case "C":
                            HandleCollect(p, mObj["CollectableID"]);
                            break;

                        case "RsP":
                            HandleRespawn(p, mObj);
                            break;

                        case "GAW":
                            HandleGetActiveWorlds(p);
                            break;

                        case "TDmg":
                            {
                                if (p != null)
                                {
                                    if (p.world != null)
                                    {
                                        BSONObject rsp = new BSONObject("UD");

                                        rsp["U"] = p.Data.UserID;
                                        rsp["x"] = p.world.SpawnPointX;
                                        rsp["y"] = p.world.SpawnPointY;
                                        rsp["DBl"] = 0;
                                        p.world.Broadcast(ref rsp);
                                        p.Send(ref mObj);
                                    }
                                }
                                break;
                            }
                        case "XPCl":
                            break;
                        case "PDC":
                            {
                                if (p != null)
                                {
                                    if (p.world != null)
                                    {
                                        BSONObject rsp = new BSONObject();
                                        rsp["ID"] = "UD";
                                        rsp["U"] = p.Data.UserID;
                                        rsp["x"] = p.world.SpawnPointX;
                                        rsp["y"] = p.world.SpawnPointY;
                                        rsp["DBl"] = 0;
                                        p.world.Broadcast(ref rsp);
                                        p.Send(ref mObj);
                                    }
                                }
                                break;
                            }

                        case "Di":
                            HandleDropItem(p, mObj);
                            break;
                        case MsgLabels.Ident.RemoveInventoryItem:
                            HandleTrashItem(p, mObj);
                            break;
                        case "mp":
                            // Not sure^^
                            break;

                        case MsgLabels.Ident.MovePlayer:
                            HandleMovePlayer(p, mObj);
                            break;



                        case MsgLabels.Ident.SetBlock:
                            HandleSetBlock(p, mObj);
                            break;

                        case MsgLabels.Ident.SetBackgroundBlock:
                            HandleSetBackgroundBlock(p, mObj);
                            break;

                        case MsgLabels.Ident.HitBlock:
                            HandleHitBlock(p, mObj);
                            break;

                        case MsgLabels.Ident.HitBackgroundBlock:
                            HandleHitBackground(p, mObj);
                            break;

                        case MsgLabels.Ident.SyncTime:
                            HandleSyncTime(client);
                            break;
                        case MsgLabels.Ident.ChangeOrb:
                            HandleOrbChange(p, mObj);
                            break;
                        case MsgLabels.Ident.ChangeWeather:
                            HandleWeatherChange(p, mObj);
                            break;
                        case MsgLabels.Ident.Summon:
                            HandleSummon(p, mObj);
                            break;
                        case MsgLabels.Ident.KickPlayer:
                            HandleKick(p, mObj);
                            break;
                        case MsgLabels.Ident.BanPlayer:
                            HandleBan(p, mObj);
                            break;
                        case MsgLabels.Ident.WorldItemUpdate:
                            HandleWorldItemUpdate(p, mObj);
                            break;
                        default:
                            pServer.OnPing(client, 1);
                            break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }



            }
        }

        private byte[] playerDataTemp = File.ReadAllBytes("player.dat").Skip(4).ToArray(); // template for playerdata, too painful to reverse rn so I am just gonna modify whats needed.
        public void HandlePlayerLogon(FeatherClient client, BSONObject bObj)
        {
#if DEBUG
            Util.Log("Handling player logon...");
#endif

            string cogID = bObj[MsgLabels.CognitoId];
            string cogToken = bObj[MsgLabels.CognitoToken];

            var resp = SimpleBSON.Load(playerDataTemp)["m0"] as BSONObject;
            var accHelper = pServer.GetAccountHelper();

            Player p = accHelper.LoginPlayer(cogID, cogToken, client.GetIPString());
            if (p == null)
            {
                Util.Log("Player was null upon logon!!");
                client.DisconnectLater();
                return;
            }

            if (p.Client == null)
            {
                Util.Log("Client was null, so setting it here!");
                p.SetClient(client);
                client.data = p.Data;
            }

            string userID = p.Data.UserID;

            if (!pServer.players.ContainsKey(userID))
            {
                pServer.players[userID] = p; // just a test with userID = 0
            }
            else
            {
                p = pServer.players[userID];

                if (p.isInGame)
                {
                    Util.Log("Account is online already, disconnecting current client!");
                    if (p.Client != null)
                    {
                        if (p.Client.isConnected())
                        {
                            p.Client.Send(new BSONObject("DR"));
                            p.Client.DisconnectLater();
                        }
                    }
                }
            }

            p.Data.CognitoID = cogID;
            p.Data.Token = cogToken;
            BSONObject pd = new BSONObject("pD");
            pd[MsgLabels.PlayerData.ByteCoinAmount] = p.Data.Coins;
            pd[MsgLabels.PlayerData.GemsAmount] = p.Data.Gems;
            pd[MsgLabels.PlayerData.Username] = p.Data.Name.ToUpper();
            pd[MsgLabels.PlayerData.PlayerOPStatus] = (int)p.Data.adminStatus;
            pd[MsgLabels.PlayerData.InventorySlots] = 400;
            pd[MsgLabels.PlayerData.ShowOnlineStatus] = true;
            pd[MsgLabels.PlayerData.ShowLocation] = true;

            // pd["experienceAmount"] = 180000;
            // pd["xpAmount"] = 180000;




            if (p.Data.Inventory.Count == 0)
            {
                p.inventoryManager.RegularDefaultInventory();
            }

            pd["xpAmount"] = 599;
            pd["experienceAmount"] = 100;
            pd["inv"] = p.inventoryManager.GetInventoryAsBinary();
            pd["tutorialState"] = 3;
            resp["rUN"] = p.Data.Name;
            resp["pD"] = SimpleBSON.Dump(pd);
            resp["U"] = p.Data.UserID;
            resp["Wo"] = "PIXELSTATION";
            resp["EmailVerified"] = true;
            resp["Email"] = p.IsUnregistered() ? "Use /register at any world" : "www.ltps.xyz";

            p.SetClient(client); // override client...
            client.data = p.Data;
            p.isInGame = true;

            client.Send(resp);
        }

        public string HandleCommandClearInventory(Player p)

        {
            p.inventoryManager.ClearInventory();
            BSONObject r = new BSONObject("DR");
            p.Send(ref r);

            return "Cleared inventory!";
        }

        public string HandleCommandGlobalMessage(Player p, string[] args)
        {
            if (args.Length < 2)
            {
                return "Usage: /gm (your message)";
            }

            string msg_query = "";

            for (int i = 1; i < args.Length; i++)
            {
                msg_query += args[i];

                if (i < args.Length - 1) msg_query += " ";
            }


            if (p.Data.Gems >= 5000)
            {
                p.RemoveGems(5000);
                BSONObject gObj = new BSONObject(MsgLabels.Ident.BroadcastGlobalMessage);
                gObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage($"<color=#C576F6>{p.Data.Name}", p.world.WorldName, p.world.WorldName, 1,
                   msg_query);

                pServer.Broadcast(ref gObj);

                return "";
            }
            else
            {
                return "Not enough gems to send a Global Message! (You need 5000 Gems to sent broadcast).";
            }
        }

        public string HandleCommandPay(Player p, string[] args)
        {
            if (args.Length < 3)
                return "Usage: /pay (name) (gems amount)";

            string user = args[1];
            int amt;
            int.TryParse(args[2], out amt);

            if (amt < 100 || amt > 999999)
            {
                return "Can only send gems between 100 and 999999!";
            }

            if (p.Data.Gems < amt)
                return "Not enough gems to transfer.";

            var player = pServer.GetOnlinePlayerByName(user);
            if (player == null)
                return String.Format("{0} is offline.", user);

            if (player == p)
                return "Cannot transfer gems to yourself, nice try!";

            p.RemoveGems(amt);
            player.AddGems(amt);

            return String.Format("Transfered {0} Gems to Account {1}!", amt, player.Data.Name);


        }

        public string HandleCommandRegister(Player p, string[] args)
        {
            if (args.Length < 3)
                return "Usage: /register (name) (password)";

            string name = args[1], pass = args[2];

            if (SQLiteManager.HasIllegalChar(name) || SQLiteManager.HasIllegalChar(pass))
                return "Username or password has illegal character! Only letters and numbers.";

            if (pass.Length > 24 || name.Length > 24 || pass.Length < 3 || name.Length < 3)
                return "Username or Password too long or too short!";

            if (!p.IsUnregistered())
                return "You are registered already!";

            var sql = pServer.GetSQL();

            using (var read = sql.FetchQuery($"SELECT * FROM players WHERE Name='{name}'"))
            {
                if (read.HasRows)
                    return "An account with this name already exists!";
            }

            if (sql.Query($"UPDATE players SET Name='{name}', Pass='{pass}' WHERE ID='{p.Data.UserID}'") > 0)
            {
                p.Data.Name = name;
                BSONObject r = new BSONObject("DR");

                p.Send(ref r);
                return "";
            }

            return "Couldn't register right now, try again!";
        }

        public string HandleCommandLogin(Player p, string[] args)
        {
            if (args.Length < 3)
                return "Usage: /login (name) (password)";

            string name = args[1], pass = args[2];

            if (SQLiteManager.HasIllegalChar(name) || SQLiteManager.HasIllegalChar(pass))
                return "Username or password has illegal character! Only letters and numbers.";

            if (pass.Length > 24 || name.Length > 24 || pass.Length < 3 || name.Length < 3)
                return "Username or Password too long or too short!";

            if (!p.IsUnregistered())
                return "You are logged on already!";

            var sql = pServer.GetSQL();
            using (var read = sql.FetchQuery($"SELECT * FROM players WHERE Name='{name}' AND Pass='{pass}'"))
            {
                string uID = "0";

                if (!read.HasRows)
                    return "Account does not exist or password is wrong! Try again?";

                if (!read.Read())
                    return "Account does not exist or password is wrong! Try again?";


                uID = (string)read["ID"];

                Util.Log("CognitoID: " + p.Data.CognitoID + " Token: " + p.Data.Token + " UID: " + uID + " UserID: " + p.Data.UserID);

                var cmd = sql.Make("UPDATE players SET CognitoID=@CognitoID, Token=@Token WHERE ID=@ID");
                cmd.Parameters.AddWithValue("@CognitoID", p.Data.CognitoID);
                cmd.Parameters.AddWithValue("@Token", p.Data.Token);
                cmd.Parameters.AddWithValue("@ID", uID);

                if (sql.PreparedQuery(cmd) > 0 && sql.Query($"DELETE FROM players WHERE ID='{p.Data.UserID}'") > 0)
                {
                    BSONObject r = new BSONObject("DR");
                    p.Client.Send(r);
                    p.Client.Flush();

                    pServer.players.Remove(p.Data.UserID);
                    return "";
                }
            }

            return "Couldn't login right now, try again!";
        }



        public void HandleWorldChatMessage(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            string msg = bObj["msg"];

            string[] tokens = msg.Split(" ");
            int tokCount = tokens.Count();

            if (tokCount <= 0)
                return;

            if (tokens[0] == "")
                return;

            if (tokens[0][0] == '/')
            {
                string res = "Unknown command.";
                switch (tokens[0])
                {
                    case "/help":
                        res = "Commands >> /help , /give (item id) , /find (item name) , /register (username pass) , /login (username pass), /pay (username amount) , /gm , /online , /givegems , /shop";
                        break;

                    case "/gm":
                        {
                            res = HandleCommandGlobalMessage(p, tokens);
                            break;
                        }

                    case "/notavaiablecsn023":
                        BSONObject gObj = new BSONObject(MsgLabels.Ident.BroadcastGlobalMessage);
                        gObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage($"<color=#00FAFA>{p.Data.Name}", p.world.WorldName, p.world.WorldName, 1,
                           String.Format("Spun the wheel and got {0}", Util.rand.Next(0, 36)));


                        p.world.Broadcast(ref gObj);
                        res = "You spin the wheel!";
                        break;

                    case "/pay":
                        {
                            res = HandleCommandPay(p, tokens);
                            break;
                        }


                    case "/find":
                        {
                            if (tokCount < 2)
                            {
                                res = "Usage: /find (item name)";
                                break;
                            }

                            string item_query = "";

                            for (int i = 1; i < tokens.Length; i++)
                            {
                                item_query += tokens[i];

                                if (i < tokens.Length - 1) item_query += " ";
                            }

                            if (item_query.Length < 2)
                            {
                                res = "Please enter an item name with more than 2 characters!";
                                break;
                            }

                            var items = ItemDB.FindByAnyName(item_query);

                            if (items.Length > 0)
                            {
                                string found = "";

                                foreach (var it in items)
                                {
                                    found += $"\nItem Name: {it.name} ID: {it.ID}";
                                }

                                res = $"Found items:{found}";
                            }
                            else
                            {
                                res = $"No item containing '{item_query}' was found.";
                            }
                            break;
                        }

                    case "/register":
                        res = HandleCommandRegister(p, tokens);
                        break;

                    case "/shop":
                        res = "Welcome to LTPS Shop, you can purchase in-game packs with gems here.\n1- Wings Pack | Purchase Command: /wingspack\n2- VIP Pack | Purchase Command: /vippack\n3- Influencer Pack | Purchase Command: /infpack\n4- Hand Pack | Purchase Command: /handpack\n5- Mask Pack | Purchase Command: /maskpack\n6- Farmable Pack | Purchase Command: /farmablepack";
                        break;

                    case "/wingspack":
                        if (p.Data.Gems >= 200000)
                        {
                            res = "Bought Wings Pack for 200.000 Gems!";
                            p.RemoveGems(200000);
                            p.inventoryManager.wingsPack();
                            BSONObject aws = new BSONObject("DR");
                            p.Send(ref aws);

                        }
                        else
                        {
                            res = "Wings Pack is 200.000 Gems. Not enough gems to purchase!\nWings Pack includes: Dark Pixie Wings , Frost Wings , Ghost Wings , Wings of the Deep , Dracula Cape , Tormentor Wings , Cthulhu Wings , Dark Ifrit Wings , Dark Sprite Wings , Scorcher Wings , Flaming Wings , Bone Wings";
                        }

                        break;



                    case "/vippack":
                        if (p.Data.Gems >= 75000)
                        {
                            res = "Bought VIP Pack for 75000 Gems!";
                            p.RemoveGems(75000);
                            p.inventoryManager.vipEsyaVer();
                            BSONObject awsa = new BSONObject("DR");
                            p.Send(ref awsa);

                        }
                        else
                        {
                            res = "VIP Pack is 75.000 Gems. Not enough gems to purchase!\nWings Pack includes: Every VIP item on shop which you are not avaiable to purchase.";
                        }

                        break;


                    case "/infpack":
                        if (p.Data.Gems >= 4000000)
                        {
                            res = "Bought Mod Pack for 4000000 Gems!";
                            p.RemoveGems(4000000);
                            p.pSettings.Set(PlayerSettings.Bit.SET_INFLUENCER);
                            BSONObject awsaa = new BSONObject("DR");
                            p.Send(ref awsaa);

                        }
                        else
                        {
                            res = "Influencer Pack is 4.000.000 Gems. Not enough gems to purchase!\nInfluencer Role Pack includes: @In-Game Influencer Role + Instant 100.000 Gems Claim";
                        }

                        break;


                    case "/handpack":
                        if (p.Data.Gems >= 150000)
                        {
                            res = "Bought Hand Pack for 150000 Gems!";
                            p.RemoveGems(150000);
                            p.inventoryManager.handPack();
                            BSONObject awsaa = new BSONObject("DR");
                            p.Send(ref awsaa);

                        }
                        else
                        {
                            res = "Hand Pack is 150.000 Gems. Not enough gems to purchase!\nHand Pack includes: Spirit Claw , Scythe , Dual Blades , Spirit Blade , Soul Cleaver , AK47 , Jake's Katana & Hilt";
                        }

                        break;


                    case "/maskpack":
                        if (p.Data.Gems >= 75000)
                        {
                            res = "Bought Mask Pack for 75000 Gems!";
                            p.RemoveGems(75000);
                            p.inventoryManager.maskPack();
                            BSONObject awsaaq = new BSONObject("DR");
                            p.Send(ref awsaaq);

                        }
                        else
                        {
                            res = "Mask Pack is 75.000 Gems. Not enough gems to purchase!\nMask Pack includes: Tormentor Mask , Dark Ifrit Mask , Dark Sprite Mask , Cthulhu Mask , Endless Mask , Flaming Mask , Scorcher Mask";
                        }

                        break;


                    case "/farmablepack":
                        if (p.Data.Gems >= 10000)
                        {
                            res = "Bought Mask Pack for 10000 Gems!";
                            p.RemoveGems(10000);
                            p.inventoryManager.farmablePack();
                            BSONObject awsaaqw = new BSONObject("DR");
                            p.Send(ref awsaaqw);

                        }
                        else
                        {
                            res = "Farmable Pack is 10.000 Gems. Not enough gems to purchase!\nFarmable Pack includes: 150 Pot of Gold (1 Block gives up to 100-150 gems each)";
                        }

                        break;







                    case "/login":
                        res = HandleCommandLogin(p, tokens);

                        break;


                    case "/online":
                        res = ($"{pServer.GetPlayersIngameCount()} players are online.");
                        break;

                    case "/givegems":
                        res = "Given 25 Gems";
                        p.AddGems(25);
                        break;



                    case "/give":
                        if (tokCount < 2)
                        {
                            res = "Usage: /give (Item ID)";
                        }
                        else
                        {
                            int id;
                            int.TryParse(tokens[1], out id);

                            var it = ItemDB.GetByID(id);

                            if (it.ID <= 0)
                            {
                                res = $"Item {id} not found!";
                            }
                            else
                            {

                                if (Shop.ContainsItem(id))
                                {
                                    res = "This item is not free! You can purchase in the /shop or its unobtainable.";
                                    break;
                                }
                                p.world.Drop(id, 20, p.Data.PosX, p.Data.PosY, ItemDB.GetByID(id).type);

                                res = @$"Given 20 {it.name}  (ID {id}).";


                            }
                        }
                        break;

                    default:
                        break;
                }

                if (res != "")
                {
                    bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FFA500>LTPS",
                        p.world.WorldName,
                        p.world.WorldName,
                        1,
                        res);

                    p.Send(ref bObj);
                }
            }
            else
            {
                bObj[MsgLabels.MessageID] = "WCM";
                bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage(p.Data.Name, p.Data.UserID, "#" + p.world.WorldName, 0, msg);
                p.world.Broadcast(ref bObj, p);
            }
        }

        public void HandleMoreWorldInfo(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            var w = pServer.GetWorldManager().GetByName(bObj["WN"]);

            bObj[MsgLabels.Count] = w == null ? 0 : w.Players.Count;
            p.Send(ref bObj);
        }

        public void HandlePlayerStatusChange(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;


            bObj["U"] = p.Data.UserID;
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleShopPurchase(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            string id = bObj["IPId"];
            Util.Log(id);
            bObj["S"] = "PS";

            if (Shop.offers.ContainsKey(id))
            {
                var s = Shop.offers[id];


                if (s.items != null)
                {
                    if (p.Data.Gems >= s.price)
                    {
                        bObj["IPRs"] = s.items.SelectMany(item => Enumerable.Repeat(item.Key, item.Value2)).ToList();

                        foreach (var item in s.items)
                        {
                            p.inventoryManager.AddItemToInventory((BlockType)item.Key, item.Value1, (short)item.Value2);
                        }

                        p.RemoveGems(s.price);
                    }
                    else
                    {
                        return;
                    }
                }
            }



            bObj["IPRs2"] = new List<int>();

            p.Send(ref bObj);
        }

        public void HandleRenamePlayer(Player p, BSONObject bObj)
        {
            string username = bObj["UN"];

            p.Send(ref bObj);
        }

        public void HandleTryToJoinWorld(Player p, BSONObject bObj, string wldName = "")
        {
            if (p == null)
            {
                Util.Log("p is null");
                return;
            }

            Util.Log($"Player with userID: {p.Data.UserID.ToString()} is trying to join a world [{pServer.GetPlayersIngameCount()} players online!]...");

            BSONObject resp = new BSONObject(MsgLabels.Ident.TryToJoinWorld);
            resp[MsgLabels.JoinResult] = (int)MsgLabels.WorldJoinResult.TooManyPlayersInWorld;
            if (bObj.ContainsKey("W"))
            {
                resp["WN"] = bObj["W"];
                resp["WB"] = 0;
            }


            var wmgr = pServer.GetWorldManager();
            string worldName = bObj["W"];

            WorldSession world = wmgr.GetByName(worldName, true);

            if (SQLiteManager.HasIllegalChar(worldName))
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.WorldJoinResult.NotValidWorldName;
            }
            else if(world.IsPlayerBanned(p))
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.WorldJoinResult.UserIsBanned;
                resp["BanState"] = "World";
                resp["T"] = world.banList[p.Data.UserID];
                resp["BPUR"] = "Hacking Suspicion";
                resp["BPl"] = 1;

            }
            else if (world != null)
            {
#if DEBUG
                Util.Log("World not null, JoinResult SUCCESS, joining world...");
#endif
                resp[MsgLabels.JoinResult] = (int)MsgLabels.WorldJoinResult.Ok;
            }
            else
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.WorldJoinResult.TooManyPlayersInWorld;
            }

            p.Send(ref resp);
        }

        public void HandleGetWorld(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            HandleLeaveWorld(p, null);

            string worldName = bObj["W"];
            var wmgr = pServer.GetWorldManager();

            WorldSession world = wmgr.GetByName(worldName, true);
            if (SQLiteManager.HasIllegalChar(worldName))
            {
                return;
            }
            else if (world.IsPlayerBanned(p))
            {
                return;
            }
            else if (world == null)
            {
                return;
            }

            world.AddPlayer(p);

            BSONObject resp = new BSONObject();
            BSONObject wObj = world.Serialize();

            resp[MsgLabels.MessageID] = MsgLabels.Ident.GetWorldCompressed;
            resp["W"] = Util.LZMAHelper.CompressLZMA(SimpleBSON.Dump(wObj));

            p.Send(ref resp);
            p.Tick();

            p.isLoadingWorld = true;
        }

        public void HandleLeaveWorld(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            BSONObject resp = new BSONObject("PL");
            resp[MsgLabels.UserID] = p.Data.UserID;

            p.world.Broadcast(ref resp, p);

            if (bObj != null)
                p.Send(ref bObj);

            p.world.RemovePlayer(p);
            p.isLoadingWorld = false;

            Util.Log($"Player with UserID {p.Data.UserID} left the world!");
        }

        public void HandleRequestOtherPlayers(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            //p.Send(ref bObj);


            long kukTime = Util.GetKukouriTime();
            foreach (var player in p.world.Players)
            {
                if (player.Data.UserID == p.Data.UserID)
                    continue;

                string prefix = "";
                switch (player.pSettings.GetHighestRank())
                {

                    case Ranks.INFLUENCER:
                        prefix = "<color=#69de05>";
                        break;

                    case Ranks.ADMIN:
                        prefix = "<color=#E744DE>";
                        break;

                    case Ranks.MODERATOR:
                        prefix = "<color=#42e2fa>";
                        break;

                    default:
                        break;
                }

                BSONObject pObj = new BSONObject("AnP");
                pObj["x"] = player.Data.PosX;
                pObj["y"] = player.Data.PosY;
                pObj["t"] = kukTime;
                pObj["a"] = player.Data.Anim;
                pObj["d"] = player.Data.Dir;
                List<int> spotsList = new List<int>();
                //spotsList.AddRange(player.GetSpots());


                pObj["spots"] = spotsList;
                pObj["familiar"] = 0;
                pObj["familiarName"] = "LTPS";
                pObj["familiarLvl"] = 0;
                pObj["familiarAge"] = kukTime;
                pObj["isFamiliarMaxLevel"] = false;
                pObj["UN"] = prefix + player.Data.Name;
                pObj["U"] = player.Data.UserID;
                pObj["Age"] = 69;
                pObj["LvL"] = 10;
                pObj["xpLvL"] = 10;
                pObj["pAS"] = 0;
                pObj["PlayerAdminEditMode"] = false;
                pObj[MsgLabels.PlayerData.PlayerOPStatus] = (int)player.pSettings.GetHighestRank();
                pObj["Ctry"] = 999;
                pObj["GAmt"] = player.Data.Gems;
                pObj["ACo"] = 0;
                pObj["QCo"] = 0;
                pObj["Gnd"] = 0;
                pObj["skin"] = 7;
                pObj["faceAnim"] = 0;
                pObj["inPortal"] = false;
                pObj["SIc"] = 0;
                pObj["D"] = 0;
                pObj["VIPEndTimeAge"] = kukTime;
                pObj["IsVIP"] = true;

                p.Send(ref pObj);
            }

            p.Send(ref bObj);
        }

        public enum GlobalMessageResult
        {
            Unknown,
            Timeout,
            ConnectionFailed,
            AuthenticationFailed,
            NoMessage,
            NoSender,
            NoGems,
            Success,
        }


        public void HandleGlobalMessage(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (p.Data.Gems >= 5000)
            {
                p.RemoveGems(5000);

                var cmb = SimpleBSON.Load(Convert.FromBase64String(bObj["msg"]));

                string msg = cmb["message"].stringValue;
                if (msg.Length > 256)
                    return;

                BSONObject gObj = new BSONObject(MsgLabels.Ident.BroadcastGlobalMessage);
                gObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage($"<color=#DA83E88>{p.Data.Name}", p.world.WorldName, p.world.WorldName, 1,
                   msg);

                pServer.Broadcast(ref gObj);
            }
        }

        public void HandleSpawnPlayer(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            long kukTime = Util.GetKukouriTime();
            BSONObject pObj = new BSONObject();
            pObj[MsgLabels.MessageID] = "AnP";
            pObj["x"] = p.Data.PosX;
            pObj["y"] = p.Data.PosY;
            pObj["t"] = kukTime;
            pObj["a"] = p.Data.Anim;
            pObj["d"] = p.Data.Dir;
            List<int> spotsList = new List<int>();
            //  spotsList.AddRange(Enumerable.Repeat(35, 35));

            string prefix = "";
            switch (p.pSettings.GetHighestRank())
            {
                case Ranks.INFLUENCER:
                    prefix = "<color=#69de05>";
                    break;

                case Ranks.ADMIN:
                    prefix = "<color=#E744DE>";
                    break;

                case Ranks.MODERATOR:
                    prefix = "<color=#42e2fa>";
                    break;

                default:
                    break;
            }


            pObj["spots"] = spotsList;
            pObj["familiar"] = 0;
            pObj["familiarName"] = "LTPS";
            pObj["familiarLvl"] = 0;
            pObj["familiarAge"] = kukTime;
            pObj["isFamiliarMaxLevel"] = true;
            pObj["UN"] = prefix + p.Data.Name;
            pObj["U"] = p.Data.UserID;
            pObj["Age"] = 69;
            pObj["LvL"] = 99;
            pObj["xpLvL"] = 99;
            pObj["pAS"] = 0;
            pObj["PlayerAdminEditMode"] = false;
            pObj["Ctry"] = 999;
            pObj["GAmt"] = p.Data.Gems;
            pObj["ACo"] = 0;
            pObj["QCo"] = 0;
            pObj["Gnd"] = 0;
            pObj["skin"] = 7;
            pObj["faceAnim"] = 0;
            pObj["inPortal"] = false;
            pObj["SIc"] = 0;
            pObj["VIPEndTimeAge"] = kukTime;
            pObj[MsgLabels.PlayerData.PlayerOPStatus] = (int)p.pSettings.GetHighestRank();
            pObj["IsVIP"] = true;

            p.world.Broadcast(ref pObj, p);

            BSONObject cObj = new BSONObject("WCM");

            cObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FFFF00>LTPS - #1 Pixel Worlds Server\nYou are able to purchase packs with gems, use /shop for more info!\nPlease login or register via commands only.\n",
                 p.world.WorldName,
                 p.world.WorldName,
                 1,
                    "--------------------\nWelcome to our server, Wanna purchase some in-game gems? Checkout: https://ltps.xyz/shop");

            p.Send(ref cObj);

            ///

        }

        public void HandleRequestAI(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            p.Send(ref bObj);
        }

        public void HandleGetActiveWorlds(Player p)
        {
            if (p == null)
                return;

            BSONObject resp = new BSONObject("GAW");
            List<string> worldNames = new List<string>();
            List<int> playerCounts = new List<int>();

            foreach (var world in pServer.GetWorldManager().GetWorlds())
            {
                int pC = world.Players.Count;
                if (pC > 0)
                {
                    worldNames.Add(world.WorldName);
                    playerCounts.Add(pC);
                }
            }

            resp["W"] = worldNames;
            resp["WN"] = worldNames;
            resp["Ct"] = playerCounts;
            p.Send(ref resp);
        }

        public void HandleRequestAIp(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            p.Send(ref bObj);
        }

        public void HandleWearableUsed(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            int id = bObj["hBlock"];

            if (id < 0 || id >= ItemDB.ItemsCount())
                return;

            Item it = ItemDB.GetByID(id);

            bObj[MsgLabels.UserID] = p.Data.UserID;
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleCollect(Player p, int colID)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (!p.world.collectables.ContainsKey(colID))
                return;

            BSONObject resp = new BSONObject();
            resp["ID"] = "C";
            resp["CollectableID"] = colID;

            WorldInterface.Collectable c = p.world.collectables[colID];
            resp["BlockType"] = c.item;
            resp["Amount"] = c.amt; // HACK
            resp["InventoryType"] = c.type;
            resp["PosX"] = c.posX;
            resp["PosY"] = c.posY;
            resp["IsGem"] = c.gemType > -1;
            resp["GemType"] = c.gemType < 0 ? 0 : c.gemType;

            if (c.gemType < 0)
            {
                p.inventoryManager.AddItemToInventory((BlockType)c.item, (InventoryItemType)c.type, c.amt);
            }
            else
            {
                int gemsToGive = 0;
                switch ((GemType)c.gemType)
                {
                    case GemType.Gem1:
                        gemsToGive = 1;
                        break;

                    case GemType.Gem2:
                        gemsToGive = 5;
                        break;

                    case GemType.Gem3:
                        gemsToGive = 20;
                        break;

                    case GemType.Gem4:
                        gemsToGive = 50;
                        break;

                    case GemType.Gem5:
                        gemsToGive = 100;
                        break;

                    default:
                        break;
                }

                p.Data.Gems += gemsToGive;
            }

            p.world.RemoveCollectable(colID, p);
            p.Send(ref resp);
        }

        public void HandleWearableRemoved(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            int id = bObj["hBlock"];

            if (id < 0 || id >= ItemDB.ItemsCount())
                return;

            Item it = ItemDB.GetByID(id);


            bObj[MsgLabels.UserID] = p.Data.UserID;
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleTryToJoinWorldRandom(Player p)
        {
            var worlds = pServer.GetWorldManager().GetWorlds();

            if (worlds.Count > 0)
            {
                var w = worlds[new Random().Next(worlds.Count)];

                BSONObject bObj = new BSONObject();
                bObj["ID"] = "OoIP";
                bObj["IP"] = "prod.gamev85.portalworldsgame.com";
                bObj["WN"] = w.WorldName;

                p.Send(ref bObj);
            }
        }

        public void HandleRespawn(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            BSONObject resp = new BSONObject();
            resp[MsgLabels.MessageID] = "UD";
            resp[MsgLabels.UserID] = p.Data.UserID;
            resp["x"] = w.SpawnPointX;
            resp["y"] = w.SpawnPointY;
            resp["DBl"] = 0;

            w.Broadcast(ref resp);
            p.Send(ref bObj);
        }
        public void HandleHitBackground(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            var tile = w.GetTile(x, y);

            BSONObject resp = new BSONObject("DB");

            if (tile != null)
            {
                if (tile.bg.id <= 0)
                    return;

                if (p.world.lockWorldData != null && ((!w.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
                {
                    p.SelfChat("World is locked by " + pServer.GetNameFromUserID(w.lockWorldData.GetPlayerWhoOwnsLockId()));
                    return;
                }

                if (Util.GetSec() > tile.bg.lastHit + 4)
                {
                    tile.bg.damage = 0;
                }

                if (++tile.bg.damage > 2)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.bg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID;
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    tile.bg.id = 0;
                    tile.fg.damage = 0;

                    double pX = x / Math.PI, pY = y / Math.PI;

                    for (int i = 0; i < 5; i++)
                        w.Drop(0, 1, pX - 0.1 + Util.rand.NextDouble(0, 0.2), pY - 0.1 + Util.rand.NextDouble(0, 0.2), 0, Util.rand.Next(3));

                }

                tile.bg.lastHit = Util.GetSec();
            }
        }

        public void HandleHitBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            var tile = w.GetTile(x, y);

            BSONObject resp = new BSONObject("DB");

            if (tile != null)
            {
                if (tile.fg.id <= 0 || tile.fg.id == 110)
                    return;

            

                if (p.world.lockWorldData != null && ((!p.world.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
                {
                    p.SelfChat("World is locked by " + pServer.GetNameFromUserID(w.lockWorldData.GetPlayerWhoOwnsLockId()));
                    return;
                }

                if (Util.GetSec() > tile.fg.lastHit + 4)
                {
                    tile.fg.damage = 0;
                }

                if (++tile.fg.damage > 2)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.fg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID;
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    double pX = x / Math.PI, pY = y / Math.PI;

                    if (tile.fg.id == (short)WorldInterface.BlockType.LockWorld)
                    {
                        w.worldItems.Remove(w.lockWorldData);
                        w.lockWorldData = null;
                        w.Drop(tile.fg.id, 1, pX, pY, 0);
                        HandleCollect(p, w.colID);
                    }
                  



                    for (int i = 0; i < 5; i++)

                        if (tile.fg.id == 762)
                        {
                            w.Drop(0, 1, pX - 0.1 + Util.rand.NextDouble(0, 0.2), pY - 0.1 + Util.rand.NextDouble(0, 0.2), 0, Util.rand.Next(4));
                        }
                        else
                            
                     w.Drop(0, 1, pX - 0.1 + Util.rand.NextDouble(0, 0.2), pY - 0.1 + Util.rand.NextDouble(0, 0.2), 0, Util.rand.Next(3));
                      tile.fg.id = 0;
                      tile.fg.damage = 0;
                      return;

            

                }



                tile.fg.lastHit = Util.GetSec();



            }

        }

        public void HandleSetBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            Item it = ItemDB.GetByID(blockType);
           
            if (blockType == 273)
                return;



            var invIt = p.inventoryManager.HasItemAmountInInventory((BlockType)blockType, (InventoryItemType)it.type);
            if (!invIt)
                return;
        
            if (p.world.lockWorldData != null && ((!p.world.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
            {
                p.SelfChat("World is locked by " + pServer.GetNameFromUserID(w.lockWorldData.GetPlayerWhoOwnsLockId()));
                return;
            }

            bObj["U"] = p.Data.UserID;
            if(it.type==3) bObj[MsgLabels.MessageID] = MsgLabels.Ident.SetBlockWater;
            bool suc = p.world.SetBlock(x, y, blockType, p);
            if (suc)
            {
                p.world.Broadcast(ref bObj);
                if (blockType == (short)BlockType.LockWorld)
                {
                    BSONObject bbobj = new BSONObject();
                    bbobj["ID"] = "WIU";
                    bbobj["WiB"] = new BSONObject();
                    bbobj["WiB"]["class"] = "LockWorldData";
                    bbobj["WiB"]["itemId"] = w.itemIndex;
                    bbobj["WiB"]["blockType"] = (int)blockType;
                    bbobj["WiB"]["animOn"] = true;
                    bbobj["WiB"]["direction"] = 0;
                    bbobj["WiB"]["anotherSprite"] = true;
                    bbobj["WiB"]["damageNow"] = false;
                    bbobj["WiB"]["playerWhoOwnsLockId"] = p.Data.UserID;
                    bbobj["WiB"]["playerWhoOwnsLockName"] = p.Data.Name;
                    bbobj["WiB"]["playersWhoHaveAccessToLock"] = new List<string>();
                    bbobj["WiB"]["playersWhoHaveMinorAccessToLock"] = new List<string>();
                    bbobj["WiB"]["isOpen"] = false;
                    bbobj["WiB"]["punchingAllowed"] = false;
                    bbobj["WiB"]["creationTime"] = DateTime.UtcNow;
                    bbobj["WiB"]["lastActivatedTime"] = DateTime.UtcNow;
                    bbobj["WiB"]["isBattleOn"] = false;
                    bbobj["x"] = x;
                    bbobj["y"] = y;
                    bbobj["ItsNewWIB"] = true;
                    w.Broadcast(ref bbobj);
                }
                p.inventoryManager.RemoveItemsFromInventory((BlockType)blockType, 0);

            }
        }

        public void HandleSetBackgroundBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            if (p.world.lockWorldData != null && ((!p.world.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
            {
                p.SelfChat("World is locked by " + pServer.GetNameFromUserID(w.lockWorldData.GetPlayerWhoOwnsLockId()));
                return;
            }

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            Item it = ItemDB.GetByID(blockType);

            var invIt = p.inventoryManager.HasItemAmountInInventory((BlockType)blockType, (InventoryItemType)1);
            if (!invIt)
                return;

            bObj["U"] = p.Data.UserID;


            var t = w.GetTile(x, y);
            t.bg.id = blockType;
            t.bg.damage = 0;
            t.bg.lastHit = 0;

            w.Broadcast(ref bObj);

            p.inventoryManager.RemoveItemsFromInventory((BlockType)blockType, (InventoryItemType)1);
        }

        public void HandleDropItem(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            BSONObject dObj = bObj["dI"] as BSONObject;

            BlockType blockType = (BlockType)dObj["BlockType"].int32Value;
            int amount = dObj["Amount"];
            int type = dObj["InventoryType"];
            double x = Convert.ToDouble(bObj["x"].int32Value) / Math.PI;
            double y = Convert.ToDouble(bObj["y"].int32Value) / Math.PI;

            var invItem = p.inventoryManager.HasItemAmountInInventory(blockType, (InventoryItemType)type, (short)amount);

            if (!invItem)
                return;

            p.inventoryManager.RemoveItemsFromInventory(blockType, (InventoryItemType)type, (short)amount);
            p.SendRemoveItemInventory(blockType, (InventoryItemType)type, amount);
            p.world.Drop((int)blockType, amount, x - 0.1 + Util.rand.NextDouble(0, 0.2), y - 0.1 + Util.rand.NextDouble(0, 0.2), type, -1);

        }
        public void HandleTrashItem(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            BSONObject dObj = bObj["dI"] as BSONObject;

            BlockType blockType = (BlockType)dObj["BlockType"].int32Value;
            int amount = dObj["Amount"];
            int type = dObj["InventoryType"];

            var invItem = p.inventoryManager.HasItemAmountInInventory(blockType, (InventoryItemType)type, (short)amount);

            if (!invItem)
                return;

            p.inventoryManager.RemoveItemsFromInventory(blockType, (InventoryItemType)type, (short)amount);
            p.SendRemoveItemInventory(blockType, (InventoryItemType)type, amount);

        }

        public void HandleMovePlayer(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (bObj.ContainsKey("x") &&
                bObj.ContainsKey("y") &&
                bObj.ContainsKey("a") &&
                bObj.ContainsKey("d") &&
                bObj.ContainsKey("t"))

            {
                p.Data.PosX = bObj["x"].doubleValue;
                p.Data.PosY = bObj["y"].doubleValue;

                p.Data.Anim = bObj["a"];
                p.Data.Dir = bObj["d"];
                bObj["U"] = p.Data.UserID;

                if (bObj.ContainsKey("tp"))
                    bObj.Remove("tp");

                p.world.Broadcast(ref bObj, p);
            }
        }

        public void HandleSyncTime(FeatherClient client)
        {
            BSONObject resp = new BSONObject(MsgLabels.Ident.SyncTime);
            resp[MsgLabels.MessageID] = MsgLabels.Ident.SyncTime;
            resp[MsgLabels.TimeStamp] = Util.GetKukouriTime();
            resp[MsgLabels.SequencingInterval] = 60;

            client.Send(resp);
        }

        public void HandleOrbChange(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            var w = p.world;

            if (p.world.lockWorldData != null && ((!w.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
            {
                p.SelfChat("You cant use orb at worlds which you dont own!");
                return;
            }
            else
            {
                int orb = bObj["bgT"].int32Value;
                BlockType blockType = Config.getOrbBlockType(orb);
                bool invItem = p.inventoryManager.HasItemAmountInInventory(blockType, InventoryItemType.Consumable);
                if (invItem)
                {
                    p.inventoryManager.RemoveItemsFromInventory(blockType, InventoryItemType.Consumable, 1);
                    p.world.BackGroundType = (LayerBackgroundType)orb;
                }
                p.SendRemoveItemInventory(blockType, InventoryItemType.Consumable, 1);
                BSONObject wObj = new BSONObject();
                wObj["ID"] = "ChangeBackground";
                wObj["bgT"] = orb;
                wObj["U"] = p.Data.UserID;
                p.world.Broadcast(ref wObj);
            }
        }
        public void HandleWeatherChange(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            var w = p.world;

            if (p.world.lockWorldData != null && ((!w.lockWorldData.DoesPlayerHaveAccessToLock(p.Data.UserID)) || w.lockWorldData.GetIsOpen()))
            {
                p.SelfChat("You cant use orb at worlds which you dont own!");
            }
            else
            {
                int weather = bObj["wto"].int32Value;
                BlockType blockType = Config.getWeatherBlockType(weather);
                bool invItem = p.inventoryManager.HasItemAmountInInventory(blockType, InventoryItemType.Consumable);
                if (invItem)
                {
                    p.inventoryManager.RemoveItemsFromInventory(blockType, InventoryItemType.Consumable, 1);
                    p.world.WeatherType = (WeatherType)weather;
                }
                p.SendRemoveItemInventory(blockType, InventoryItemType.Consumable, 1);
                BSONObject wObj = new BSONObject();
                wObj["ID"] = "CWWoq";
                wObj["wto"] = weather;
                wObj["U"] = p.Data.UserID;
                p.world.Broadcast(ref wObj);
            }

        }
        public void HandleSummon(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            bool perm = p.world.CanSummon(p, bObj["U"].stringValue);
            if (!perm) return;

            //var pos = Config.ConvertWorldPointToMapPoint(Convert.ToSingle(p.Data.PosX), Convert.ToSingle(p.Data.PosY));
            BSONObject mObj = new BSONObject();
            mObj["ID"] = "WP";
            mObj["U"] = bObj["U"].stringValue;
            mObj["PX"] = Convert.ToInt32((float)p.Data.PosX * Math.PI);
            mObj["PY"] = Convert.ToInt32((float)p.Data.PosY * Math.PI);
            p.world.Broadcast(ref mObj);


        }
        public void HandleKick(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            bool perm = p.world.CanKick(p, bObj["U"].stringValue);
            if (!perm) return;

            BSONObject mObj = new BSONObject();           
            mObj["ID"] = "KPl";
            mObj["BPl"] = 0;
            mObj["WN"] = p.world.WorldName;
            mObj["BanState"] = "World";
            mObj["T"] = DateTime.UtcNow.Ticks;
            mObj["BanFromGameReasonValue"] = "Scamming";
            //mObj["Idx"] = 0;
            Player player = p.world.Players.Find(pl => pl.Data.UserID == bObj["U"].stringValue);
            if (player.Data.UserID == bObj["U"].stringValue)
            {
                player.Send(ref mObj);
                //p.world.RemovePlayer(player);
            }
        }
        public void HandleBan(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            bool perm = p.world.CanBan(p, bObj["U"].stringValue);
            if (!perm) return;

            BSONObject mObj = new BSONObject();
            mObj["ID"] = "KPl";
            mObj["BPl"] = 1;
            mObj["WN"] = p.world.WorldName;
            mObj["BanState"] = "World";
            mObj["T"] = DateTime.UtcNow.Ticks;
            mObj["BanFromGameReasonValue"] = "Scamming";
            //mObj["Idx"] = 0;
            Player player = p.world.Players.Find(pl => pl.Data.UserID == bObj["U"].stringValue);
            if (player.Data.UserID == bObj["U"].stringValue)
            {
                player.Send(ref mObj);
                player.world.banList.Add(player.Data.UserID, DateTime.UtcNow.AddHours(1).Ticks);
                //p.world.RemovePlayer(player);
            }
        }
        public void HandleWorldItemUpdate(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;
            WorldItemBase worldItem = p.world.FindItemBaseWithID(bObj["WiB"]["itemId"].int32Value);
            if(worldItem == null) return;
            if(worldItem.blockType == BlockType.LockWorld)
            {
                var worldLock = (LockWorldData)worldItem;

                if (p.Data.UserID != worldLock.GetPlayerWhoOwnsLockId()) return;
                if (p.Data.Name != worldLock.GetPlayerWhoOwnsLockName())worldLock.SetPlayerWhoOwnsLockName(p.Data.Name);
                foreach (string str in bObj["WiB"]["playersWhoHaveAccessToLock"].stringListValue)
                {
                    if (!IsAccessFormatValid(str)) return;
                }
                foreach (string str in bObj["WiB"]["playersWhoHaveMinorAccessToLock"].stringListValue)
                {
                    if (!IsAccessFormatValid(str)) return;
                }
                worldLock.SetPlayersWhoHaveAccessToLock(bObj["WiB"]["playersWhoHaveAccessToLock"].stringListValue);
                worldLock.SetPlayersWhoHaveMinorAccessToLock(bObj["WiB"]["playersWhoHaveMinorAccessToLock"].stringListValue);
                worldLock.SetIsOpen(bObj["WiB"]["isOpen"].boolValue);
                worldLock.SetIsPunchingAllowed(bObj["WiB"]["punchingAllowed"].boolValue);
                BSONObject wObj = new BSONObject();
                wObj["ID"] = "WIU";
                wObj["WiB"] = p.world.lockWorldData.GetAsBSON();
                wObj["x"] = p.world.lockWorldData.x;
                wObj["y"] = p.world.lockWorldData.y;
                wObj["PT"] = 1;
                wObj["U"] = p.Data.UserID;
                p.world.Broadcast(ref wObj);

            }
        }
        public static bool IsAccessFormatValid(string input)
        {
            string pattern = @"^\w+;\w+$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(input);
        }
        public delegate void LogDelegate(string message);

        public static void ReadBSON(BSONObject SinglePacket, string Parent = "", LogDelegate Log = null)
        {
            if (Log == null)
            {
                Log = Util.Log;
            }
            foreach (string Key in SinglePacket.Keys)
            {
                try
                {

                    BSONValue Packet = SinglePacket[Key];
                    if (Key == "ID" && Packet.stringValue == "p") return;
                    switch (Packet.valueType)
                    {
                        case BSONValue.ValueType.String:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.stringValue}");
                            break;
                        case BSONValue.ValueType.Boolean:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.boolValue}");
                            break;
                        case BSONValue.ValueType.Int32:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.int32Value}");
                            break;
                        case BSONValue.ValueType.Int64:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.int64Value}");
                            break;
                        case BSONValue.ValueType.Binary: // BSONObject
                            try
                            {
                                Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.binaryValue}");
                                ReadBSON(SimpleBSON.Load(Packet.binaryValue), Key);
                            }
                            catch
                            {
                                Log($"{Parent} = {Key} | {Packet.valueType} = [{BitConverter.ToString(Packet.binaryValue)}]");
                            }
                            break;
                        case BSONValue.ValueType.Double:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.doubleValue}");
                            break;
                        case BSONValue.ValueType.Array:
                            string bamboom = $"{Parent} = {Key} | {Packet.valueType} = " + "[" + string.Join(", ", Packet.stringListValue) + "]";
                            Log(bamboom);
                            break;
                        case BSONValue.ValueType.UTCDateTime:
                            Log($"{Parent} = {Key} | {Packet.valueType} = {Packet.dateTimeValue}");
                            break;
                        default:
                            Log($"{Parent} = {Key} | {Packet.valueType}");
                            ReadBSON((BSONObject)Packet, Key, Log);
                            //Log(BitConverter.ToString(ObjectToByteArray(((Object)Packet))));

                            break;
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
                }
            }
        }
    }
}

﻿using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using Timer = System.Timers.Timer;
using System.IO;

namespace PixelWorldsServer2.Networking.Server
{
    public class PWServer
    {
        private readonly Timer tickTimer = new Timer(FeatherDefaults.PING_CLOCK_MS);
        public bool wantsShutdown = false;
        public int Version = 96;
        public int Port; // for quick-accessibility
        private FeatherServer fServer = null;
        private MessageHandler msgHandler = null;
        private SQLiteManager sqlManager = null;
        private WorldManager worldManager = null;
        private AccountHelper accountHelper = null;
        public Dictionary<uint, Player> players = new Dictionary<uint, Player>();
        public object locker = new object();
        private long lastDiscordUpdateTime;
        public FeatherServer GetServer() => fServer;
        public MessageHandler GetMessageHandler() => msgHandler;
        public WorldManager GetWorldManager() => worldManager;
        public AccountHelper GetAccountHelper() => accountHelper;

        // return null if non existent:
        public Player GetPlayerByUserID(uint userID)
        {
            Player p = null;

            if (players.ContainsKey(userID))
            {
                p = players[userID];
            }
            else
            {
                var pSQL = GetSQL();

                var reader = pSQL.FetchQuery("SELECT * FROM players WHERE ID='" + userID.ToString() + "'");
                if (reader.Read())
                {
                    p = new Player(reader);
                    players[p.Data.UserID] = p;
                }
            }

            return p;
        }

        public string GetNameFromUserID(uint userID)
        {
            var p = GetPlayerByUserID(userID);

            return p == null ? "DeletedUser" : p.Data.Name;
        }

        public Player GetOnlinePlayerByName(string name)
        {
            string nameLower = name.ToLower();
            foreach (var p in players.Values)
            {
                if (!p.IsOnline())
                    continue;

                if (p.Data.Name.ToLower() == nameLower)
                    return p;
            }

            return null;
        }

        public Player GetOnlinePlayerByUserID(uint userID)
        {
            foreach (var p in players.Values)
            {
                if (!p.IsOnline())
                    continue;

                if (p.Data.UserID == userID)
                    return p;
            }

            return null;
        }

        private void HandleConsoleGiveGems(uint userID, int amount)
        {
            if (amount == 0)
            {
                Util.Log("Error can't use null amount!");
                return;
            }

            var p = GetOnlinePlayerByUserID(userID);
            if (p == null)
            {
                Util.Log("This user isn't online. (Aborted)");
                return;
            }

            if (amount < 0)
            {
                amount = -amount; // reverse the negativity with another negativity so that it actually removes positive gems again.
                p.RemoveGems(amount); 

                Util.Log(String.Format("Removed {0} Gems from Account {1} (ID: {2})", amount, p.Data.Name, userID));
            }
            else
            {
                p.AddGems(amount);
                Util.Log(String.Format("Given {0} Gems to Account {1} (ID: {2})", amount, p.Data.Name, userID));
            }

           
        }
        private static void GetWorldThread(object obj)
        {
            object[] parameters = (object[])obj;
            RealPW.Client getWorldClient = (RealPW.Client)parameters[0];
            getWorldClient.GetWorldData((string)parameters[1]);
        }

        [Obsolete]
        private void HandleConsoleCloneWorld(string WorldName)
        {
            if(!SQLiteManager.HasIllegalChar(WorldName))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Please wait while the server clones world data.");
                Console.ForegroundColor = ConsoleColor.White;
                RealPW.Client getWorldClient = new RealPW.Client();
                Thread newThread = new Thread(new ParameterizedThreadStart(GetWorldThread));
                object[] parameters = { getWorldClient, WorldName};
                newThread.Start(parameters);
                int timeWaited = 0;
                while((getWorldClient.taskStatus != "Finished" && getWorldClient.taskStatus != "Failed"))
                {
                    Thread.Sleep(3000);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Stay patience while we clone world.");
                    Console.ForegroundColor = ConsoleColor.White;
                    timeWaited = timeWaited + 3000;
                    if (timeWaited > 120000)
                    {
                        break;
                    }
                }
                if(getWorldClient.taskStatus == "Finished")
                {
                    MessageHandler.ReadBSON(getWorldClient.WorldData);
                    SaveFromBSON(getWorldClient.WorldData, WorldName);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to clone world, try again or fix code.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                RealPW.Client.TCP.stream.Close();
                RealPW.Client.TCP.socket.Close();
                newThread.Suspend();
            }
            else
            {
                Util.Log("World name has invalid character(s).");
            }

            //44.194.163.69
        }

        public static void SaveFromBSON(BSONObject bobj, string WorldName)
        {
            int worldsizeX = bobj["WorldSizeSettingsType"]["WorldSizeX"].int32Value;
            int worldsizeY = bobj["WorldSizeSettingsType"]["WorldSizeY"].int32Value;
            string path = $"maps/{WorldName}.map";
            WorldTile[,] tilos = new WorldTile[worldsizeX, worldsizeY];
            int tileLen = tilos.Length;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x1); // version
                uint ownerid = 0;
                ms.Write(BitConverter.GetBytes(ownerid));

                int pos = 5;
                for (int i = 0; i < bobj["BlockLayer"].binaryValue.Length; i = i + 2)
                {
                    ms.Write(bobj["BlockLayer"].binaryValue, i, 2);
                    ms.Write(bobj["BackgroundLayer"].binaryValue, i, 2);
                    ms.Write(bobj["WaterLayer"].binaryValue, i, 2);
                    ms.Write(bobj["WiringLayer"].binaryValue, i, 2);

                    pos += 2;
                }
                ms.Write(BitConverter.GetBytes(bobj["Collectables"]["Count"].int32Value));
                for (int i = 0; i < bobj["Collectables"]["Count"].int32Value; i++)
                {
                    BSONObject col = (BSONObject)bobj["Collectables"]["C" + i];
                    ms.Write(BitConverter.GetBytes((Int16)col["BlockType"].int32Value));
                    ms.Write(BitConverter.GetBytes((Int16)col["Amount"].int32Value));
                    ms.Write(BitConverter.GetBytes(col["PosX"].doubleValue));
                    ms.Write(BitConverter.GetBytes(col["PosY"].doubleValue));
                    short gemType = 0;
                    if (!col["IsGem"].boolValue)
                    {
                        gemType = -1;
                    }
                    if(gemType > -1)
                    {
                        gemType = (Int16)col["GemType"].int32Value;
                    }
                    ms.Write(BitConverter.GetBytes(gemType));
                }
                File.WriteAllBytes(path, Util.LZMAHelper.CompressLZMA(ms.ToArray()));
                SpinWait.SpinUntil(() => Util.IsFileReady(path));
                Console.WriteLine("Saved");
            }
        }
        private void HandleConsoleSetRank(uint userID, Ranks rankType)
        {
            // duration is in secs here...
            var p = GetOnlinePlayerByUserID(userID);
            if (p == null)
            {
                Util.Log("This user isn't online. Use 'getinfo <name>' if you want to grab the userID of a player's name. (Aborted)");
                return;
            }

            switch (rankType)
            {
                case Ranks.ADMIN:
                    p.pSettings.Set(PlayerSettings.Bit.SET_ADMIN);
                    break;

                case Ranks.INFLUENCER:
                    p.pSettings.Set(PlayerSettings.Bit.SET_INFLUENCER);
                    break;

                case Ranks.MODERATOR:
                    p.pSettings.Set(PlayerSettings.Bit.SET_MOD);
                    break;

                default:
                    break;
            }

            Util.Log("User rank has been set! Will request this user to reconnect...");
            
            BSONObject r = new BSONObject("DR");
            p.Send(ref r);
        }

        private void HandleConsoleGetInfo(string username)
        {
            Util.Log("Obtaining player info from username '" + username + "'...");

            var timeMs = Util.GetMs();
            var reader = sqlManager.FetchQuery("SELECT * FROM players WHERE Name='" + username + "'");

            if (reader.Read())
            {
                Player player = new Player(reader);

                uint userID = player.Data.UserID;
                if (players.ContainsKey(userID))
                {
                    player = players[userID];
                }

                Util.Log($"Result:  UserID: {userID}, Gems: {player.Data.Gems}, IP: {player.Data.LastIP}, Online: " + (player.isInGame ? $"yes (in '{player.GetWorldName()}')" : "no"));
            }
            else
            {
                Util.Log("No record(s) found.");
            }

            Util.Log($"Search took {Util.GetMs() - timeMs} milliseconds to perform.");
        }

        public void ConsoleCommand(string[] cmd)
        {
            // Process the console input command:

            lock (locker)
            {
                try
                {
                    switch (cmd[0])
                    {
                        case "?":
                        case "help":
                            Util.Log("Commands: setvip, setmod, setadmin, getinfo, stop");
                            break;

                        case "stop":
                            this.Shutdown();
                            break;

                        case "getinfo":
                            if (cmd.Length > 1)
                                HandleConsoleGetInfo(cmd[1]);

                            break;

                        case "setvip":
                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.INFLUENCER);

                            break;

                        case "setmod":
                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.MODERATOR);

                            break;

                        case "setadmin":

                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.ADMIN);

                            break;

                        case "givegems":

                            if (cmd.Length > 2)
                                HandleConsoleGiveGems(uint.Parse(cmd[1]), int.Parse(cmd[2]));

                            break;
                        case "cloneworld":
                            if (cmd.Length > 1)
                                HandleConsoleCloneWorld(cmd[1].ToUpper());
                            break;
                        default:
                            Util.Log("Unknown command. Type 'help' for a list of commands.");
                            break;
                    }
                }
                catch(Exception e) { Util.Log("Invalid arguments!"); Console.WriteLine(e); }
            }
        }

        public void Shutdown()
        {
            try
            {
                Util.Log("Server is shutting down...");

                // will call destructors:
                long ms = Util.GetMs();

                //fServer.Stop();
                sqlManager.Close();
                worldManager.SaveAll();
                worldManager.Clear();

                foreach (var p in players.Values)
                    p.Save();

                players.Clear();
                tickTimer.Stop();

                Util.Log($"Shutdown finished in {Util.GetMs() - ms} ms.");

                wantsShutdown = true;
                GC.KeepAlive(this);
            }
            catch (Exception ex)
            {
                Util.Log(ex.Message);
            }
        }

        public int GetPlayersIngameCount()
        {
            int c = 0;

            foreach (Player player in players.Values)
            {
                if (player.isInGame)
                    c++;
            }

            return c; // should be a more optimized version of returning the entire list.
        }

        public List<Player> GetPlayersIngame()
        {
            List<Player> ingame = new List<Player>();

            foreach (Player player in players.Values)
            {
                if (player.isInGame)
                    ingame.Add(player);
            }

            return ingame;
        }

        [Obsolete]
        public PWServer(int port = 10001)
        {
            Port = port;
            fServer = new FeatherServer(Port);
            sqlManager = new SQLiteManager();
            msgHandler = new MessageHandler(this);
            worldManager = new WorldManager(this);
            accountHelper = new AccountHelper(this);
        }
        public SQLiteManager GetSQL() { return sqlManager; }

        public bool Start()
        {
            bool started = fServer == null ? false : fServer.Start();

            if (started)
            {
                tickTimer.AutoReset = true;
                tickTimer.Elapsed += Tick;
                tickTimer.Start();
            }

            return started;
        }

        public void Broadcast(ref BSONObject bObj, params Player[] ignored)
        {
            foreach (var p in players.Values)
            {
                if (ignored.Contains(p))
                    continue;

                if (p.isInGame)
                {
                    p.Send(ref bObj);
                }
            }
        }

        public void Tick(object obj, ElapsedEventArgs e)
        {
            lock (locker)
            {
                int playersOn = 0;
                foreach (var p in players.Values)
                {
                    if (p.isInGame)
                    {
                        playersOn++;
                        
                        if (!p.isLoadingWorld)
                            p.Tick();
                    }
                }

                var clients = fServer.GetClients();
                foreach (var client in clients)
                {
                    if (client.areWeSending)
                    {
                        OnPing(client, 1);
                        client.Flush();
                    }
                }

                //worldManager.CheckAll();

                if (Util.GetSec() > lastDiscordUpdateTime + 29)
                {
                    _ = DiscordBot.UpdateStatus($"Join {playersOn} other players!");
                    lastDiscordUpdateTime = Util.GetSec();
                }
            }
        }

        public bool Poll(int duration = 1)
        {
            return fServer.GetListener().Server.Poll(duration * 1000, SelectMode.SelectRead);
        }

        public void Host()
        {
            bool sleep = false;
            lock (locker)
            {
                var evs = fServer.Service(1);
                if (evs.Length == 0)
                    sleep = true;

                foreach (var ev in evs)
                {
                    switch (ev.type)
                    {
                        case FeatherEvent.Types.CONNECT:
                            OnConnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.DISCONNECT:
                            OnDisconnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.RECEIVE:
                            try
                            {
                                OnReceive(ev.client, SimpleBSON.Load(ev.packetData), ev.flags);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("Don't know elementType"))
                                    ev.client.DisconnectLater();
                            }
                            break;

                        case FeatherEvent.Types.PING_NOW:
                            break;

                        default:
                            break;
                    }
                }
            }

            if (sleep)
                Thread.Sleep(1);
        }

        // onPing is used for other stuff too so it's public here...
        public void OnPing(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            Player p = client.data == null ? null : ((Player.PlayerData)client.data).player;
            
            if (p == null)
                return;

            if (flags == 0)
            {
                p.sendPing = true; // unused for now
            }
            else
            {
                p.SendPing();
            }
        }

        private void OnDisconnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            if (client.data == null)
                return;

            var pData = (Player.PlayerData)client.data;
            // depends on whether we were the last instance to disconnect with that userID:
            // have to this as the player might try to relogon onto the same session.
            ushort instances = 0;
            foreach (FeatherClient fClient in fServer.GetClients())
            {
                if (fClient.data == null)
                    continue;

                if (((Player.PlayerData)fClient.data).UserID == pData.UserID)
                    instances++;
            }

            Player p = pData.player;
            p.isInGame = instances > 0;

            if (!p.isInGame)
            {
                Util.Log("Player nowhere ingame anymore, unregistering session...");

                GetMessageHandler().HandleLeaveWorld(p, null);
                p.SetClient(null);
            }
        }

        private void OnReceive(FeatherClient client, BSONObject packet, int flags)
        {
            if (client == null)
                return;

            msgHandler.ProcessBSONPacket(client, packet);
            lock(client.sendLock){
            client.areWeSending = true;
            }
        }

        private void OnConnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            client.StartReading(this);
        }
    }
}

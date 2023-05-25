﻿using System;
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

namespace PixelWorldsServer2
{
    public class Player
    {
        private PWServer pServer = null;
        public PlayerSettings pSettings = null;
        public bool isInGame = false; // when the player has logon and is inside.
        public bool sendPing = false;
        public bool isLoadingWorld = false;
        public bool IsOnline() => isInGame && Client != null;
        public struct PlayerData
        {
            public Player player;

            public uint UserID;
            public int Gems, Coins;
            public string CognitoID, Token;
            public string Name;
            public string LastIP;
            public PlayerInventory Inventory;
            public double PosX, PosY;
            public int Anim, Dir;
            public AdminStatus adminStatus;
            public BSONObject BSON;
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
                pData.UserID = 0;
                pData.Gems = 0;
                pData.Coins = 0;
                pData.CognitoID = "";
                pData.Token = "";
                pData.Name = "";
                pData.LastIP = "0.0.0.0";
                pData.Inventory = new PlayerInventory();
                pData.BSON = new BSONObject();

                fClient.data = pData; // interlink
            }
        }
        public Player(SQLiteDataReader reader)
        {
            pData.player = this;
            this.pSettings = new PlayerSettings((int)reader["Settings"]);
            pData.UserID = (uint)(long)reader["ID"];
            pData.Gems = (int)reader["Gems"];
            pData.Coins = (int)reader["ByteCoins"];
            pData.Name = (string)reader["Name"];
            pData.LastIP = (string)reader["IP"];
            pData.adminStatus = (Player.AdminStatus)reader["AdminStatus"];

            object inven = reader["Inventory"];
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

            pData.Inventory = new PlayerInventory(invData);
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

            byte[] invData = Data.Inventory.Serialize();
            cmd.Parameters.Add("@Inventory", DbType.Binary);
            cmd.Parameters["@Inventory"].Value = invData;

            cmd.Parameters.AddWithValue("@ID", Data.UserID);

            if (sql.PreparedQuery(cmd) > 0)
            {
                //Util.Log($"Player ID: {Data.UserID} ('{Data.Name}') saved.");
            }
        }
    }
}

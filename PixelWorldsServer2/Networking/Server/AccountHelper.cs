using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static PixelWorldsServer2.Player;

namespace PixelWorldsServer2.Networking.Server
{
    public class AccountHelper
    {
        PWServer pServer = null;
        public AccountHelper(PWServer pServer)
        {
            this.pServer = pServer;
        }

        // forceRegister: Register Player if not found in Database. If false, null can be returned.
        public Player LoginPlayer(string cogID, string cogToken, string ip, bool forceRegister = true)
        {
            Player player = null;
            var sql = pServer.GetSQL();

            var cmd = sql.Make("SELECT * FROM players WHERE CognitoID=@CognitoID AND Token=@Token");

            cmd.Parameters.AddWithValue("@CognitoID", cogID);
            cmd.Parameters.AddWithValue("@Token", cogToken);

            using (var reader = sql.PreparedFetchQuery(cmd))
            {
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        // Found an entry:
                        player = new Player(reader);
                    }
                    else
                    {
                        player = CreateAccount(cogID, cogToken, ip);
                    }
                }
                else
                {
                    player = CreateAccount(cogID, cogToken, ip);
                }
            }

            return player;
        }
        public Player CreateAccount(string cogID, string cogToken, string ip = "0.0.0.0", int adminStatus = 0)
        {

            var sql2 = pServer.GetSQL();
            bool validID = false;
            int count = 20;
            string ID = "";
            while (count > 0 && !validID) {
                try
                {
                    var cmdt = sql2.Make("SELECT * FROM players WHERE ID=@ID");
                    ID = Util.RandomString(8);
                    cmdt.Parameters.AddWithValue("@ID", ID);

                    using (var reader = sql2.PreparedFetchQuery(cmdt))
                    {
                        if (reader != null)
                        {
                            if (reader.Read())
                            { count--; continue; }

                            else
                            {
                                validID = true;
                                continue;
                            }
                        }
                        else
                        {
                            validID = true;
                            continue;
                        }
                    }
                }
                catch { count--; }
            }
            if (count == 0) return null;

            var sql = pServer.GetSQL();

            var cmd = sql.Make("INSERT INTO players (ID, Name, CognitoID, Token, IP, AdminStatus, RecentWorlds) VALUES (@ID, @Name, @CognitoID, @Token, @IP, @AdminStatus, @RecentWorlds)");

            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.Parameters.AddWithValue("@CognitoID", cogID);
            cmd.Parameters.AddWithValue("@Token", cogToken);
            cmd.Parameters.AddWithValue("@IP", ip);
            cmd.Parameters.AddWithValue("@AdminStatus", adminStatus);
            cmd.Parameters.AddWithValue("@RecentWorlds", "");

            string name = "LTPS_" + Util.RandomString(8); // Name generation soon...
            cmd.Parameters.AddWithValue("@Name", name);

            if (sql.PreparedQuery(cmd) > 0)
            {
                var p = new Player();
                p.Data.player = p;
                p.Data.UserID = ID;
                p.Data.Inventory = new Dictionary<int, short>();
                p.Data.CognitoID = cogID;
                p.Data.Token = cogToken;
                p.Data.Name = name;
                p.Data.LastIP = ip;
                p.pSettings = new PlayerSettings();
                p.Data.adminStatus = (Player.AdminStatus)adminStatus;
                p.inventoryManager = new PlayerInventoryManager(p);
                p.inventoryManager.RegularDefaultInventory();
                p.Data.recentWorlds = new RecentWorlds();

                return p;
            }

            return null;
        }
        public bool UpdatePlayer(int playerId, string name = null, int? cogID = null, string cogToken = null, string ip = null, int? adminStatus = null)
        {
            var sql = pServer.GetSQL();

            try
            {
                var cmd = sql.Make("UPDATE players SET " +
                                   (name != null ? "Name = @Name, " : "") +
                                   (cogID.HasValue ? "CognitoID = @CognitoID, " : "") +
                                   (cogToken != null ? "Token = @Token, " : "") +
                                   (ip != null ? "IP = @IP, " : "") +
                                   (adminStatus.HasValue ? "AdminStatus = @AdminStatus, " : "") +
                                   "WHERE ID = @PlayerID");

                if (name != null)
                    cmd.Parameters.AddWithValue("@Name", name);

                if (cogID.HasValue)
                    cmd.Parameters.AddWithValue("@CognitoID", cogID.Value);

                if (cogToken != null)
                    cmd.Parameters.AddWithValue("@Token", cogToken);

                if (ip != null)
                    cmd.Parameters.AddWithValue("@IP", ip);

                if (adminStatus.HasValue)
                    cmd.Parameters.AddWithValue("@AdminStatus", adminStatus.Value);

                cmd.Parameters.AddWithValue("@PlayerID", playerId);

                int rowsAffected = sql.PreparedQuery(cmd);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle the error here (e.g., log the exception or throw a custom exception)
                Console.WriteLine("An error occurred while updating the player: " + ex.Message);
                return false;
            }
        }


    }
}

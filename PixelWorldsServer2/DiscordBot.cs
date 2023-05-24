using Discord;
using Discord.WebSocket;
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
using System.Threading.Tasks;

namespace PixelWorldsServer2
{
    public class DiscordBot
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private const string token = "MTA3MzcxNzc1OTkxMDg3NTE4Ng.GqArpS.iG6ZdYesOWqTgu1kDGe1hNrm0_nC-LWYtASe64";
        public static bool hasInit = false;


        public static async Task UpdateStatus(string status)
        {
            await _client.SetGameAsync("LTPS is UP! Join the server now.");
        }


        private static Task InternalLog(LogMessage msg)
        {
            return Task.CompletedTask;        }




        private static Task Connected()
        {
            Util.Log("Discord Bot connected successfully!");
            return Task.CompletedTask;
        }

        public static void Init()
        {
            _client.Connected += Connected;
            _client.Log += InternalLog;
            hasInit = true;

            _ = Login();
        }

        public static async Task Login()
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;
using BattleNET;
using System.Net;

namespace Arma2Blacklist
{
    class Program
    {
        private static NameValueCollection settings;
        private static BeClient beclient;
        private static String serverName;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Blacklister");
            settings = ConfigurationManager.AppSettings;
            serverName = Settings.GetValue("misc", "name");
            try
            {
                beclient = new BeClient();
                beclient.beMessageParser.IpConReceived += beMessageParser_IpConReceived;
                beclient.beMessageParser.GuidConReceived += beMessageParser_GuidConReceived;
                beclient.beMessageParser.LGuidConReceived += beMessageParser_LGuidConReceived;
                beclient.beMessageParser.KickReceived += beMessageParser_KickReceived;
                beclient.beMessageParser.ChatReceived += beMessageParser_ChatReceived;
                beclient.beMessageParser.DisconnReceived += beMessageParser_DisconnReceived;
                beclient.Connect();
                do
                {
                    Thread.Sleep(1000);
                } while (beclient.IsConnected() == true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error: {0}. Exiting...", ex.Message));
                Environment.Exit(1);
            }
            Thread.Sleep(5000);
            Environment.Exit(0);
        }

        static void beMessageParser_DisconnReceived(BeMessageDisconnect msg)
        {
            Console.WriteLine(String.Format("Player Disconnected: {0} {1}", msg.player_id, msg.player_name));
        }

        static void beMessageParser_ChatReceived(BeMessageChat msg)
        {
            Console.WriteLine(String.Format("[{0}] {1}: {2}", msg.type, msg.player_name, msg.message));
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    NameValueCollection postparams = new NameValueCollection();
                    postparams.Add("player", msg.player_name);
                    postparams.Add("type", msg.type);
                    postparams.Add("message", msg.message);
                    postparams.Add("server", serverName);
                    webclient.UploadValues(Settings.GetValue("misc", "chaturl"), "POST", postparams);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Adding Chat: " + ex.Message);
            }
        }

        static void beMessageParser_KickReceived(BeMessageKick msg)
        {
            Console.WriteLine(String.Format("{0} ({1}) was kick: {2}", msg.player_name, msg.player_guid, msg.message));
        }

        static void beMessageParser_LGuidConReceived(BeMessageLGuidConnect msg)
        {
            Console.WriteLine(String.Format("LegacyGUID: {0} {1}", msg.player_name, msg.guid));
            Int16 ban_id = checkban(msg.guid, msg.player_name);
            if (ban_id > 0)
            {
                beclient.kickPlayer(msg.player_id, ban_id);
                return;
            }
        }

        static void beMessageParser_GuidConReceived(BeMessageGuidConnect msg)
        {
            Int16 ban_id = checkban(msg.guid, msg.player_name);
            if (ban_id > 0)
            {
                beclient.kickPlayer(msg.player_id, ban_id);
            }
        }

        static void beMessageParser_IpConReceived(BeMessageIpConnect msg)
        {
            Int16 ban_id = checkban(msg.ip, msg.player_name);
            if (ban_id > 0)
            {
                beclient.kickPlayer(msg.player_id, ban_id);
            }
        }

        private static Int16 checkban(String guid, String player)
        {
            String response = "0";
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    NameValueCollection postparams = new NameValueCollection();
                    postparams.Add("guid", guid);
                    postparams.Add("player", player);
                    postparams.Add("server", serverName);
                    response = Encoding.UTF8.GetString(webclient.UploadValues(Settings.GetValue("misc", "banurl"), "POST", postparams));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Checking Ban: " + ex.Message);
            }
            return Convert.ToInt16(response);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using BattleNET;
using System.Net;

namespace Arma2Blacklist
{
    class BeClient
    {
        private NameValueCollection settings;
        private BattlEyeClient be;
        private BattlEyeLoginCredentials config;
        private Int16 connectionAttempts = 0;
        private Int16 maxConnectionAttempts = 10;
        private Int16 waitTime = 1000;
        public BeMessageParser beMessageParser;

        internal BeClient()
        {
            beMessageParser = new BeMessageParser();
            settings = ConfigurationManager.AppSettings;
            config = new BattlEyeLoginCredentials
            {
                Host = IPAddress.Parse(Settings.GetValue("server", "host").ToString()),
                Port = Convert.ToInt32(Settings.GetValue("server", "port").ToString()),
                Password = Settings.GetValue("server", "password").ToString()
            };
            be = new BattlEyeClient(config);
            be.BattlEyeMessageReceived += be_MessageReceivedEvent;
            be.BattlEyeDisconnected += be_DisconnectEvent;
            be.ReconnectOnPacketLoss = true;
        }

        internal void Connect()
        {
            if (connectionAttempts >= maxConnectionAttempts)
            {
                throw new Exception("Maximum connections attempts reached");
            }
            try
            {
                connectionAttempts++;
                be.Connect();
                if (!be.Connected)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine(String.Format("Connection attempt {0} of {1} failed, waiting {2} seconds.", connectionAttempts, maxConnectionAttempts, (waitTime / 1000)));
                Thread.Sleep(waitTime);
                Connect();
            }
        }

        void be_MessageReceivedEvent(BattlEyeMessageEventArgs args)
        {
            beMessageParser.Parse(args.Message);
        }

        void be_DisconnectEvent(BattlEyeDisconnectEventArgs args)
        {
            Console.WriteLine("Disconnected");
            Environment.Exit(0);
        }

        internal bool IsConnected()
        {
            return be.Connected;
        }

        public void kickPlayer(Int16 player_id, Int16 ban_id)
        {
            String kickMsg = String.Format(Settings.GetValue("misc", "kickmessage"), ban_id);
            be.SendCommand(BattlEyeCommand.Kick, String.Format("{0} {1}", player_id, kickMsg));
        }
    }
}

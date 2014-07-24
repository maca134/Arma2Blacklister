using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arma2Blacklist
{
    class BeMessageParser
    {
        public delegate void ChatReceivedEventHandler(BeMessageChat msg);
        public event ChatReceivedEventHandler ChatReceived;

        public delegate void IpConReceivedEventHandler(BeMessageIpConnect msg);
        public event IpConReceivedEventHandler IpConReceived;

        public delegate void GuidConReceivedEventHandler(BeMessageGuidConnect msg);
        public event GuidConReceivedEventHandler GuidConReceived;

        public delegate void LGuidConReceivedEventHandler(BeMessageLGuidConnect msg);
        public event LGuidConReceivedEventHandler LGuidConReceived;

        public delegate void DisconnReceivedEventHandler(BeMessageDisconnect msg);
        public event DisconnReceivedEventHandler DisconnReceived;

        public delegate void KickReceivedEventHandler(BeMessageKick msg);
        public event KickReceivedEventHandler KickReceived;

        private Regex chatrx = new Regex(@"\((?<type>[a-zA-Z]+)\)\s(?<name>.+): (?<message>.+)", RegexOptions.IgnoreCase);
        private Regex ipconrx = new Regex(@"Player #(?<id>[0-9]+) (?<name>.*) \((?<ip>[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+):[0-9]+\) connected", RegexOptions.IgnoreCase);
        private Regex guidconrx = new Regex(@"Player #(?<id>[0-9]+) (?<name>.*) \- GUID: (?<guid>[0-9a-z]+) \(unverified\)", RegexOptions.IgnoreCase);
        private Regex lguidconrx = new Regex(@"Player #(?<id>[0-9]+) (?<name>.*) - Legacy GUID: (?<guid>[0-9a-z]+)", RegexOptions.IgnoreCase);
        private Regex disconrx = new Regex(@"Player #(?<id>[0-9]+) (?<name>.*) disconnected", RegexOptions.IgnoreCase);
        private Regex kickrx = new Regex(@"Player #(?<id>[0-9]+) (?<name>.*) \((?<guid>[0-9a-z]+)\) has been kicked by BattlEye: (?<message>.+)", RegexOptions.IgnoreCase);

        public void Parse(String message)
        {
            Match chatMatch = chatrx.Match(message);
            if (chatMatch.Success)
            {
                parseChatMessage(chatMatch);
                return;
            }

            Match ipconMatch = ipconrx.Match(message);
            if (ipconMatch.Success)
            {
                parseIpConn(ipconMatch);
                return;
            }

            Match guidconMatch = guidconrx.Match(message);
            if (guidconMatch.Success)
            {
                parseGuidConn(guidconMatch);
                return;
            }

            Match lguidconMatch = lguidconrx.Match(message);
            if (lguidconMatch.Success)
            {
                parseLGuidConn(lguidconMatch);
                return;
            }

            Match disconMatch = disconrx.Match(message);
            if (disconMatch.Success)
            {
                parseDisConn(disconMatch);
                return;
            }

            Match kickMatch = kickrx.Match(message);
            if (kickMatch.Success)
            {
                parseKick(kickMatch);
                return;
            }

            Console.WriteLine(message);
        }

        private void parseKick(Match match)
        {
            if (KickReceived == null)
                return;
            KickReceived(new BeMessageKick
            {
                player_id = Convert.ToInt16(match.Groups["id"].Value),
                player_name = match.Groups["name"].Value.Trim(),
                player_guid = match.Groups["guid"].Value,
                message = match.Groups["message"].Value
            });

        }

        private void parseDisConn(Match match)
        {
            if (DisconnReceived == null)
                return;
            DisconnReceived(new BeMessageDisconnect
            {
                player_id = Convert.ToInt16(match.Groups["id"].Value),
                player_name = match.Groups["name"].Value.Trim()
            });
        }

        private void parseLGuidConn(Match match)
        {
            if (LGuidConReceived == null)
                return;
            LGuidConReceived(new BeMessageLGuidConnect
            {
                player_id = Convert.ToInt16(match.Groups["id"].Value),
                player_name = match.Groups["name"].Value.Trim(),
                guid = match.Groups["guid"].Value
            });
        }

        private void parseGuidConn(Match match)
        {
            if (GuidConReceived == null)
                return;
            GuidConReceived(new BeMessageGuidConnect
            {
                player_id = Convert.ToInt16(match.Groups["id"].Value),
                player_name = match.Groups["name"].Value.Trim(),
                guid = match.Groups["guid"].Value
            });
        }

        private void parseIpConn(Match match)
        {
            if (IpConReceived == null)
                return;
            IpConReceived(new BeMessageIpConnect
            {
                player_id = Convert.ToInt16(match.Groups["id"].Value),
                player_name = match.Groups["name"].Value.Trim(),
                ip = match.Groups["ip"].Value
            });
        }

        private void parseChatMessage(Match match)
        {
            if (ChatReceived == null)
                return;
            ChatReceived(new BeMessageChat
            {
                type = match.Groups["type"].Value,
                player_name = match.Groups["name"].Value.Trim(),
                message = match.Groups["message"].Value
            });
        }
    }

    class BeMessageException : Exception
    {
        public BeMessageException() { }
        public BeMessageException(string message) : base(message) { }
        public BeMessageException(string message, Exception inner) : base(message, inner) { }
    }
}

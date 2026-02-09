using System;
using System.Collections.Generic;

namespace SocketSignalServer
{
    public class ClientInfo
    {
        public string Name;
        public List<MessageDestinationInfo> MessageDestinationsList;

        public bool TimeoutCheck;
        public int TimeoutLength;
        public string TimeoutMessage;
        public DateTime LastAccessTime;
        public DateTime LastTimeoutDetectedTime;

        /// <summary>
        /// Line format:
        ///   clientName \t bool timeoutCheck \t int timeoutLength \t string timeoutMessage
        /// </summary>
        public ClientInfo(string Line, List<MessageDestinationInfo> messageDestinationsList)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Line))
                    throw new ArgumentException("Line is empty");

                var cols = Line.Split(new[] { '\t' }, StringSplitOptions.None);

                if (cols.Length < 4)
                    throw new FormatException("Insufficient columns");

                Name = cols[0];
                TimeoutCheck = bool.TryParse(cols[1], out var b) ? b : false;
                TimeoutLength = int.TryParse(cols[2], out var t) ? t : 0;
                TimeoutMessage = cols[3] ?? "";

                MessageDestinationsList = messageDestinationsList;
            }
            catch
            {
                // Fail-safe initialization
                Name = "";
                TimeoutCheck = false;
                TimeoutLength = 0;
                TimeoutMessage = "";
                MessageDestinationsList = null;
            }

            LastAccessTime = DateTime.Now;
            LastTimeoutDetectedTime = DateTime.MinValue;
        }
    }

    public class DestinationsBook
    {
        private readonly Dictionary<string, MessageDestinationInfo> destinationsDictionary;

        /// <summary>
        /// Lines example:
        ///   "192.168.1.11\tTower1"
        ///   "192.168.1.12\tTower2"
        /// </summary>
        public DestinationsBook(string[] lines)
        {
            destinationsDictionary = new Dictionary<string, MessageDestinationInfo>();

            if (lines == null)
                return;

            for (int i = 0; i < lines.Length; i++)
            {
                var key = (i + 1).ToString();

                // Skip invalid or duplicate entries safely
                if (!destinationsDictionary.ContainsKey(key))
                {
                    try
                    {
                        destinationsDictionary.Add(key, new MessageDestinationInfo(lines[i]));
                    }
                    catch
                    {
                        // Skip invalid line
                    }
                }
            }
        }

        /// <summary>
        /// keyIndexList example: "1,2,3" or "ALL"
        /// </summary>
        public List<MessageDestinationInfo> getDestinations(string keyIndexList)
        {
            if (string.IsNullOrWhiteSpace(keyIndexList) || keyIndexList == "ALL")
            {
                // Faster than foreach
                return new List<MessageDestinationInfo>(destinationsDictionary.Values);
            }

            var result = new List<MessageDestinationInfo>();
            var keyIndexSet = keyIndexList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var key in keyIndexSet)
            {
                if (destinationsDictionary.TryGetValue(key, out var dest))
                {
                    result.Add(dest);
                }
            }

            return result;
        }
    }

    public class MessageDestinationInfo
    {
        public string Address;
        public string Name;

        /// <summary>
        /// Line example: "192.168.1.11\tTower1"
        /// </summary>
        public MessageDestinationInfo(string Line)
        {
            try
            {
                var cols = Line.Split(new[] { '\t' }, StringSplitOptions.None);
                Address = cols.Length > 0 ? cols[0] : "";
                Name = cols.Length > 1 ? cols[1] : "";
            }
            catch
            {
                Address = "";
                Name = "";
            }
        }

        public override string ToString()
        {
            return Address + ":" + Name;
        }
    }
}
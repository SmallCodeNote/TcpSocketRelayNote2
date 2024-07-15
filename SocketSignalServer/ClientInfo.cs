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

        /// <summary></summary>
        /// <param name="Line">string clientName + "\t" + bool timeoutCheck + "\t" + int timeoutLength + "\t" + string timeoutMessage</param>
        /// <param name="addressList"></param>
        public ClientInfo(string Line, List<MessageDestinationInfo> MessageDestinationsList)
        {
            string[] cols = Line.Split('\t');
            try
            {
                Name = cols[0];
                TimeoutCheck = bool.Parse(cols[1]);
                TimeoutLength = int.Parse(cols[2]);
                TimeoutMessage = cols[3];
                this.MessageDestinationsList = MessageDestinationsList;
            }
            catch
            {
                Name = "";
                TimeoutCheck = false;
                TimeoutLength = 0;
                TimeoutMessage = "";
                this.MessageDestinationsList = null;
            }

            LastAccessTime = DateTime.Now;
            LastTimeoutDetectedTime = DateTime.MinValue;
        }
    }

    public class DestinationsBook
    {
        private Dictionary<string, MessageDestinationInfo> destinationsDictionary;

        /// <summary> </summary>
        /// <param name="Lines">ex) {"192.168.1.11\tTower1","192.168.1.12\tTower2",...}</param>
        public DestinationsBook(string[] Lines)
        {
            destinationsDictionary = new Dictionary<string, MessageDestinationInfo>();
            for (int i = 1; i <= Lines.Length; i++)
            {
                destinationsDictionary.Add(i.ToString(), new MessageDestinationInfo(Lines[i - 1]));
            }
        }

        /// <summary> </summary>
        /// <param name="keyIndexList">ex) "1,2,3" / "ALL" </param>
        /// <returns></returns>
        public List<MessageDestinationInfo> getDestinations(string keyIndexList)
        {
            if (keyIndexList == "" || keyIndexList == "ALL")
            {
                List<MessageDestinationInfo> result = new List<MessageDestinationInfo>();

                foreach (var addValue in destinationsDictionary)
                {
                    result.Add(addValue.Value);
                }
                return result;
            }
            else
            {
                string[] keyIndexSet = keyIndexList.Split(',');
                List<MessageDestinationInfo> result = new List<MessageDestinationInfo>();

                foreach (string key in keyIndexSet)
                {
                    if (destinationsDictionary.ContainsKey(key)) result.Add(destinationsDictionary[key]);
                }
                return result;
            }
        }
    }

    public class MessageDestinationInfo
    {
        public string Address;
        public string Name;

        /// <summary> </summary>
        /// <param name="Line">ex) "192.168.1.11\tTower1"</param>
        public MessageDestinationInfo(string Line)
        {
            string[] cols = Line.Split('\t');
            Address = cols[0];
            Name = cols.Length > 1 ? cols[1] : "";
        }

        public override string ToString()
        {
            return Address + ":" + Name;
        }
    }
}

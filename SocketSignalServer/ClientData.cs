using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    public class ClientData
    {
        public string clientName;
        public int timeoutLength;
        public string timeoutMessage;
        public List<AddressInfo> addressList;

        public bool timeoutCheck;

        public DateTime lastAccessTime;
        public DateTime lastTimeoutDetectedTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Line">clientName + "\t" + timeoutCheck + "\t" + timeoutLength + "\t" + timeoutMessage</param>
        /// <param name="addressList"></param>
        public ClientData(string Line, List<AddressInfo> addressList)
        {
            string[] cols = Line.Split('\t');

            try
            {
                clientName = cols[0];
                timeoutCheck = bool.Parse(cols[1]);
                timeoutLength = int.Parse(cols[2]);
                timeoutMessage = cols[3];
                this.addressList = addressList;
            }
            catch
            {
                clientName = "";
                timeoutCheck = false;
                timeoutLength = 0;
                timeoutMessage = "";
                this.addressList = null;
            }

            lastAccessTime = DateTime.Now;
            lastTimeoutDetectedTime = DateTime.MinValue;
        }
    }

    public class AddressBook
    {
        Dictionary<string, AddressInfo> addressBook;

        public AddressBook(string[] Lines)
        {
            addressBook = new Dictionary<string, AddressInfo>();
            for (int i = 1; i <= Lines.Length; i++)
            {
                addressBook.Add(i.ToString(), new AddressInfo(Lines[i - 1]));
            }
        }

        public List<AddressInfo> getAddress(string keyIndexList)
        {
            if (keyIndexList == "" || keyIndexList == "ALL")
            {
                List<AddressInfo> result = new List<AddressInfo>();

                foreach (var addValue in addressBook)
                {
                    result.Add(addValue.Value);
                }
                return result;
            }
            else
            {
                string[] keyIndexSet = keyIndexList.Split(',');
                List<AddressInfo> result = new List<AddressInfo>();

                foreach (string key in keyIndexSet)
                {
                    if (addressBook.ContainsKey(key)) result.Add(addressBook[key]);
                }
                return result;
            }
        }
    }

    public class AddressInfo
    {
        public string address;
        public string addressName;

        public AddressInfo(string Line)
        {
            string[] cols = Line.Split('\t');

            address = cols[0];
            addressName = cols.Length > 1 ? cols[1] : "";
        }

        public override string ToString()
        {
            return address + ":" + addressName;
        }
    }
}

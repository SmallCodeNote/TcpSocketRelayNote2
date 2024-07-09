using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    public class SocketMessage
    {
        public string clientName { get; set; }
        public DateTime connectTime { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public bool check { get; set; }
        /// <summary>
        /// Once/Ever
        /// </summary>
        public string checkStyle { get; set; }
        /// <summary>
        /// SignalParameter(/Options)
        /// </summary>
        public string parameter { get; set; }

        public SocketMessage(DateTime connectTime, string clientName, string status, string message, string parameter, string checkStyle = "Once")
        {
            this.connectTime = connectTime;
            this.clientName = clientName;
            this.status = status;
            this.message = message;
            this.check = false;
            this.checkStyle = checkStyle;
            this.parameter = parameter;
        }

        public SocketMessage()
        {
            this.connectTime = DateTime.Now;
            this.clientName = "";
            this.status = "";
            this.message = "";
            this.check = false;
            this.checkStyle = "Once";
            this.parameter = "";
        }

        public void Update(SocketMessage socketMessage)
        {
            this.connectTime = socketMessage.connectTime;
            this.clientName = socketMessage.clientName;
            this.status = socketMessage.status;
            this.message = socketMessage.message;
            this.check = socketMessage.check;
            this.checkStyle = socketMessage.checkStyle;
            this.parameter = socketMessage.parameter;
        }

        public string Key
        {
            get
            {
                return clientName + "_" + connectTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
        }

        public override string ToString()
        {
            return clientName + "\t" + connectTime.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + status + "\t" + message + "\t" + check.ToString() + "\t" + checkStyle;
        }
    }
}

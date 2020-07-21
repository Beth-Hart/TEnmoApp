using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public decimal TransferAmount { get; set; }
        public TransferType TransferType { get; set; }
        public TransferStatus TransferStatus { get; set; } = TransferStatus.Pending;
        public string FromUserName {get; set;}
        public string ToUserName {get; set;}
        public int TransferID { get; set; }

    }

    public enum TransferType
    {
        Request, Send
    }

    public enum TransferStatus
    {
        Pending, Approved, Rejected
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        bool AddTransferRequest(Transfer transfer);
        List<Transfer> GetTransfers(string username);
        Transfer GetTransferByTransferID(int transferID);
        bool UpdateTransferStatus(Transfer transfer);
    }
}

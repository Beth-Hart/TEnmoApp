
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly ITransferDAO transferDAO;

        public TransferController(IAccountDAO accountDAO, ITransferDAO transferDAO)
        {
            this.accountDAO = accountDAO;
            this.transferDAO = transferDAO;
        }

        [HttpPost("request")]
        public ActionResult<bool> AddTransferRequest(Transfer transfer)
        {
            Account fromAccount, toAccount;

            if (transfer.TransferType == TransferType.Send)
            {
                toAccount = accountDAO.GetAccount(transfer.ToUserName);
                if (toAccount == null) return NotFound();

                fromAccount = accountDAO.GetAccount(User.Identity.Name);

                if (fromAccount.Balance < transfer.TransferAmount) { return BadRequest(); }
                else
                {
                    transfer.TransferStatus = TransferStatus.Approved;
                    transfer.FromUserID = fromAccount.UserId;
                }
            }
            else
            {
                fromAccount = accountDAO.GetAccount(transfer.FromUserName);
                if (fromAccount == null) return NotFound();

                toAccount = accountDAO.GetAccount(User.Identity.Name);
                transfer.TransferStatus = TransferStatus.Pending;
                transfer.ToUserID = toAccount.UserId;
            }

            bool requestSucceeded = transferDAO.AddTransferRequest(transfer);
            if (!requestSucceeded) { return StatusCode(500); }

            CompleteTransferIfApproved(transfer);

            return Ok();
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            var transfers = transferDAO.GetTransfers(User.Identity.Name);
            if (transfers == null)
            {
                return NotFound();
            }
            return Ok(transfers);
        }

        [HttpPut]
        public ActionResult UpdateTransferStatus(Transfer transfer)
        {
            Transfer databaseTransfer = transferDAO.GetTransferByTransferID(transfer.TransferID);
            databaseTransfer.TransferStatus = transfer.TransferStatus;
            if(databaseTransfer.FromUserName != User.Identity.Name)
            {
                return Unauthorized();
            }
            else
            {
                if (databaseTransfer.TransferStatus == TransferStatus.Approved)
                {
                    Account fromAccount = accountDAO.GetAccount(User.Identity.Name);
                    if (fromAccount.Balance < transfer.TransferAmount) { return BadRequest(); }
                }
                transferDAO.UpdateTransferStatus(databaseTransfer);
                ActionResult transferResult = CompleteTransferIfApproved(databaseTransfer);
                return transferResult;
            }

        }


        private ActionResult CompleteTransferIfApproved(Transfer transfer)
        {
            if (transfer.TransferStatus == TransferStatus.Approved)
            {
                bool transferSucceeded = accountDAO.DoTransfer(transfer);
                if (!transferSucceeded) return StatusCode(500);
            }
            return Ok();
        }

    }
}
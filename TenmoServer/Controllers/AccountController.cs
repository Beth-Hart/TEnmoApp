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
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly ITransferDAO transferDAO;

        public AccountController(IAccountDAO accountDAO, ITransferDAO transferDAO)
        {
            this.accountDAO = accountDAO;
            this.transferDAO = transferDAO;
        }

        [HttpGet]
        public ActionResult<decimal> GetAccountBalance()
        {
            string userIDString = (User.FindFirst("sub")?.Value);
            int userID;
            bool parsedOK = Int32.TryParse(userIDString, out userID);
            if(!parsedOK) { return StatusCode(500); }

            decimal? balance = accountDAO.GetBalance(userID);

            if (balance == null) return NotFound();
            return Ok(balance);
        }

    }
}

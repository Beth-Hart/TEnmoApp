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
    public class UserController : ControllerBase
    {
        private readonly IUserDAO userDAO;
        private readonly IAccountDAO accountDAO;

        public UserController(IUserDAO userDAO, IAccountDAO accountDAO)
        {
            this.userDAO = userDAO;
            this.accountDAO = accountDAO;
        }

        [HttpGet("all")]
        public ActionResult<List<User>> GetUsers()
        {
            var users = userDAO.GetUsers();
            if (users == null)
            {
                NotFound();
            }
            return Ok(users);
        }

        [HttpGet("others")]
        public ActionResult<List<User>> GetOtherUsers()
        {
            Account account = accountDAO.GetAccount(User.Identity.Name);

            if (account == null) return NotFound();

            List<User> users = userDAO.GetOtherUsers(account.UserId);
            if (users == null)
            {
                NotFound();
            }
            return Ok(users);
        }

    }


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace TenmoServer.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        [Required(ErrorMessage = "You need to pass the account ID.")]
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}

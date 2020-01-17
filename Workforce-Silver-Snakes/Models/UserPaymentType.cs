using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Silver_Snakes.Models
{
    public class UserPaymentType
    {
        public int Id { get; set; }

        [Display(Name = "Account Number")]
        public int AcctNumber { get; set; }

        public bool Active { get; set; }

        public int CustomerId { get; set; }

        public int PaymentTypeId { get; set; }
        
    }
}

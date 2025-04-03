using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketOrderFunction
{
    public class TicketOrder
    {

        //concert id
        public int? ConcertId { get; set; }

        //email
        public string Email { get; set; } = string.Empty;

        //name
        public string Name { get; set; } = string.Empty;

        //phone
        public string Phone { get; set; } = string.Empty;

        //quantity
        public int Quantity { get; set; }

        //credit card
        public string CreditCard { get; set; } = string.Empty;

        //expiration date
        public string ExpirationDate { get; set; } = string.Empty;

        //security code
        public string SecurityCode { get; set; } = string.Empty;

        //address
        public string Address { get; set; } = string.Empty;

        //city
        public string City { get; set; } = string.Empty;

        //province
        public string Province { get; set; } = string.Empty;

        //postal code
        public string PostalCode { get; set; } = string.Empty;

        //country
        public string Country { get; set; } = string.Empty;
    }
}

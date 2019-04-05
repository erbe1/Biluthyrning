using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyrBil.Models
{
    public class Booking
    {
        public Guid Id { get; set; }

        [Display(Name = "Bokningsdatum")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Bokning avslutad")]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "Kostnad")]
        public decimal Cost { get; set; }

        [Display(Name = "Pågående uthyrning")]
        public bool BookingStatus { get; set; }



        public Customer Customer { get; set; }
        public Car Car { get; set; }

        private static decimal baseDayRental = 100;
        private static decimal kmPrice = 10;

        [NotMapped]
        public decimal theBaseDayRental
        {
            get { return baseDayRental; }
            //set { theBaseDayRental = baseDayRental; }
        }

        [NotMapped]
        public decimal theKmPrice
        {
            get { return kmPrice; }
        }

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyrBil.Models.ViewModels
{
    public class CarBookingVM
    {
        public Car Car { get; set; }
        public Booking Booking { get; set; }
        public Customer Customer { get; set; }
        public List<SelectListItem> CarSizes { get; set; }
        public List<SelectListItem> Customers { get; set; }
        public IEnumerable<Booking> CustomerBookings { get; set; }
    }
}

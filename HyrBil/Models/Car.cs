using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyrBil.Models
{
    public class Car
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Du måste ange ett registreringsnummer")]
        [RegularExpression(@"^[a-zA-Z]{3}[\d]{3}$", ErrorMessage = "Ogiltigt format")]
        [Display(Name = "Registreringsnummer")]
        //[RegularExpression(@"^[^iqvIQV&&^'\'d]{3}['\'d]{3}$", ErrorMessage = "Felaktig inmatning av registreringsnummer")]

        public string RegNr { get; set; }

        [Display(Name = "Storlek")]
        public CarSize CarSize { get; set; }

        [Required(ErrorMessage = "Du måste ange hur långt bilen har gått")]
        [Display(Name = "Mätarställning")]
        [RegularExpression(@"[\d]{1,10}", ErrorMessage = "Har bilen verkligen gått så långt..?")]
        public decimal CurrentDistance { get; set; }

        public decimal Mileage { get; set; }

        [Display(Name = "Uthyrd")]
        public bool Rented { get; set; }

    }

    public enum CarSize
    {
        Kompakt,
        Skåpbil,
        Minibuss
    };
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HyrBil.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }

        [Display(Name = "Förnamn")]
        [Required(ErrorMessage = "Ange kundens förnamn.")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        [Required(ErrorMessage = "Ange kundens efternamn.")]
        public string LastName { get; set; }

        [Display(Name = "Kundens bokningar")]
        public List<Booking> Bookings { get; set; }

        [Required(ErrorMessage = "Du måste ange ett personnummer.")]
        [Display(Name = "Personnummer")]
        [RegularExpression("^(19|20)?[0-9]{2}(0[1-9]|1[0-2])(0[0-9]|1[0-9]|2[0-9]|3[0-1])-?[0-9]{4}$", ErrorMessage = "Ange 10 eller 12 siffror. Ex: XXXXYYZZ-SSSS eller XXYYZZ-SS")]
        [ValidateOfAge]
        public string PNr { get; set; }
    }

    internal class ValidateOfAge : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validation)
        {
            if (value != null)
            {

                string PnrRegEx = @"^(?<date>\d{6}|\d{8})[-\s]?\d{4}$";
                string date = Regex.Match((string)value, PnrRegEx).Groups["date"].Value;
                DateTime dateTime;
                if (DateTime.TryParseExact(date, new[] { "yyMMdd", "yyyyMMdd" }, new CultureInfo("sv-SE"), DateTimeStyles.None, out dateTime))
                {
                    if (OldEnough(dateTime))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("För att hyra en bil måste kunden vara över 18 år.");
                    }
                }
                return new ValidationResult("Ange 10 eller 12 siffror.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        private bool OldEnough(DateTime dateTime)
        {
            DateTime date = DateTime.Today;
            int age = date.Year - dateTime.Year;
            if (dateTime > date.AddYears(-age))
            {
                age--;
            }
            return age < 18 ? false : true;
        }
    }
}

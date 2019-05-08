using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HyrBil.Data;
using HyrBil.Models;
using HyrBil.Models.ViewModels;
using HyrBil.Services.Repositories;

namespace HyrBil.Views
{
    public class BookingsController : Controller
    {
        private readonly IBookingsRepo _bookingsRepo;
        private readonly ICustomersRepo _customersRepo;
        private readonly ICarsRepo _carsRepo;

        public BookingsController(IBookingsRepo bookingsRepo, ICustomersRepo customersRepo, ICarsRepo carsRepo)
        {
            _bookingsRepo = bookingsRepo;
            _customersRepo = customersRepo;
            _carsRepo = carsRepo;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {

            return View(await (Task.Run(() => _bookingsRepo.GetBookings())));
        }

        // GET: Bookings/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = _bookingsRepo.GetBookingsById(id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            CarBookingVM carBookingVM = new CarBookingVM();
            carBookingVM.CarSizes = GetCarSizeToList();
            carBookingVM.Customers = GetCustomersToList();

            return View(carBookingVM);
        }

        private List<SelectListItem> GetCustomersToList()
        {
            var theCustomers = _bookingsRepo.GetCustomers();

            List<SelectListItem> customerList = new List<SelectListItem>();

            foreach (var customer in theCustomers)
            {
                string fullCustomerName = $"{customer.FirstName} {customer.LastName}";
                var c = new SelectListItem() { Text = fullCustomerName, Value = customer.CustomerId.ToString() };
                customerList.Add(c);
            }
            return customerList;
        }

        private List<SelectListItem> GetCarSizeToList()
        {
            string[] carSizeArray = Enum.GetNames(typeof(CarSize));
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            foreach (var carSize in carSizeArray)
            {
                var c = new SelectListItem() { Text = carSize, Value = carSize };
                selectListItems.Add(c);
            }
            return selectListItems;
        }


        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Customer,Car,BookingDate")] Booking booking)
        {
            //För at kontrollera så att bilen inte bokas ett datum som redan varit.
            var currentDate = DateTime.Now;

            //Väljer ut första lediga bil som matchar önskad storlek.  
            var allCars = _bookingsRepo.GetAllCars();
            var notRentedCars = allCars.Where(x => x.Rented == false).ToList();
            var theCarWeChoose = notRentedCars.FirstOrDefault(x => x.CarSize == booking.Car.CarSize);

            //För att fylla på dropdown-listan ifall det inte skulle gå att boka en bil.
            CarBookingVM carBookingVM = new CarBookingVM();
            carBookingVM.CarSizes = GetCarSizeToList();
            carBookingVM.Customers = GetCustomersToList();


            if (theCarWeChoose == null)
            {
                ViewBag.UserMessageFail = "Tyvärr fanns det inga lediga bilar av den storleken.";

                return View("Create", carBookingVM);
            }
            else if (currentDate > booking.BookingDate)
            {
                ViewBag.UserMessageFail = "Det går inte att boka ett datum som redan har varit.";

                return View("Create", carBookingVM);
            }
            else
            {
                booking.Car = theCarWeChoose;
                booking.Id = Guid.NewGuid();
                booking.BookingStatus = true;
                booking.Car.Rented = true;
                await _bookingsRepo.AddBooking(booking);

                theCarWeChoose.Rented = true;

                return RedirectToAction(nameof(Index));

            }
        }

        private object getAvailableCar(Booking booking)
        {
            var allCars = _bookingsRepo.GetAllCars();

            var notRentedCars = allCars.Where(x => x.Rented == false).ToList();
            var theCarWeChoose = notRentedCars.FirstOrDefault(x => x.CarSize == booking.Car.CarSize);
            return theCarWeChoose;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateCost([Bind("Id,Customer,Car,BookingDate,ReturnDate")] Booking booking)
        {
            var carToReturn = _bookingsRepo.GetCarToReturn(booking);
            decimal distance = booking.Car.Mileage;
            carToReturn.Car.Mileage = distance;

            var theReturnDate = booking.ReturnDate;
            carToReturn.ReturnDate = theReturnDate;

            //För att räkna ut antalet dagar bilen var uthyrd
            var daysRented = (theReturnDate - carToReturn.BookingDate).TotalDays;

            //För att konvertera till decimal
            decimal numberOfDaysRented = Convert.ToDecimal(daysRented);

            //För att avrunda till 2 decimaler
            decimal theDaysTheCarWasRented = decimal.Round(numberOfDaysRented, 2, MidpointRounding.AwayFromZero);


            if (carToReturn.Car.CarSize == CarSize.Kompakt)
            {
                carToReturn.Cost = decimal.Round((booking.theBaseDayRental * numberOfDaysRented), 2, MidpointRounding.AwayFromZero);
                if (daysRented == 1)
                {
                    ViewBag.TheCost = $"Kostnaden för {theDaysTheCarWasRented} dag blir: {carToReturn.Cost} kr";
                    await _bookingsRepo.SaveChanges();
                    return View("PayCar", carToReturn);
                }
                else
                {
                    ViewBag.TheCost = $"Kostnaden för {theDaysTheCarWasRented} dagar blir: {carToReturn.Cost} kr";
                    await _bookingsRepo.SaveChanges();
                    return View("PayCar", carToReturn);
                }

            }
            else if (carToReturn.Car.CarSize == CarSize.Skåpbil)
            {
                carToReturn.Cost = decimal.Round((booking.theBaseDayRental * numberOfDaysRented * 1.2m) + (booking.theKmPrice * distance), 2, MidpointRounding.AwayFromZero);
                ViewBag.TheCost = $"Kostnaden för {theDaysTheCarWasRented} dagar blir: {carToReturn.Cost} kr";
                await _bookingsRepo.SaveChanges();
                return View("PayCar", carToReturn);
            }
            else if (carToReturn.Car.CarSize == CarSize.Minibuss)
            {
                carToReturn.Cost = decimal.Round((booking.theBaseDayRental * numberOfDaysRented * 1.7m) + (booking.theKmPrice * distance * 1.5m), 2, MidpointRounding.AwayFromZero);
                ViewBag.TheCost = $"Kostnaden för {theDaysTheCarWasRented} dagar blir: {carToReturn.Cost} kr";
                await _bookingsRepo.SaveChanges();
                return View("PayCar", carToReturn);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakePayment(Guid id, [Bind("Id,Customer,Car,BookingDate,ReturnDate,Mileage")] Booking booking)
        {
            var finishBooking = _bookingsRepo.FinishBooking(booking);

            //Bilen är återigen tillgänglig att bokas
            finishBooking.Car.Rented = false;

            //Sätter hur långt bilen totalt har kört efter bokning
            decimal distance = finishBooking.Car.Mileage;
            finishBooking.Car.CurrentDistance = finishBooking.Car.CurrentDistance + distance;
            finishBooking.BookingStatus = false;

            //Bilen flaggas för städning
            finishBooking.Car.Cleaning = true;

            await _bookingsRepo.Update(finishBooking);
            ViewBag.Thanks = "Tack för din betalning";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult ReturnCar(Booking booking)
        {
            var carToReturn = _bookingsRepo.GetCarToReturn(booking);
            return View(carToReturn);
        }


        // GET: Bookings/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var booking = await _context.Bookings.FindAsync(id);
            var booking = _bookingsRepo.GetBookingsById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,BookingDate,ReturnDate,Cost")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingsRepo.Update(booking);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = _bookingsRepo.GetBookingsById(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            //var booking = await _context.Bookings.FindAsync(id);
            //_context.Bookings.Remove(booking);
            //await _context.SaveChangesAsync();

            var booking = _bookingsRepo.GetBookingsById(id);
            await _bookingsRepo.DeleteBooking(booking);

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(Guid id)
        {
            return _bookingsRepo.Exists(id);
            //return _context.Bookings.Any(e => e.Id == id);
        }
    }
}

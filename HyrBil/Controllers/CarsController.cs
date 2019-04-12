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
    public class CarsController : Controller
    {
        private readonly ICarsRepo _carsRepo;

        public CarsController(ICarsRepo carsRepo)
        {
            _carsRepo = carsRepo;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            return View(await (Task.Run(() => _carsRepo.GetCars())));
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

        //GET: Cars/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var car =  _carsRepo.GetCarById(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            CarBookingVM size = new CarBookingVM();
            size.CarSizes = GetCarSizeToList();
            return View(size);
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegNr,CarSize,CurrentDistance")] Car car)
        {

            var carsInDb = _carsRepo.GetCarsByRegNr(car.RegNr);

            if (carsInDb == null)
            {

                ViewBag.UserMessage = $"Bil tillagd med registreringsnummer {car.RegNr.ToUpper()}";
                ModelState.Clear();
                car.Id = Guid.NewGuid();
                _carsRepo.CreateCar(car);
                return RedirectToAction(nameof(Index));

            }
            else
            {
                ViewBag.UserMessageFail = $"Det finns redan en bil med registreringsnummer {car.RegNr.ToUpper()}";
                return RedirectToAction(nameof(Index));

            }
            //return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car =  _carsRepo.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }

            CarBookingVM carBookingVM = new CarBookingVM();
            carBookingVM.CarSizes = GetCarSizeToList();
            carBookingVM.Car = car;

            return View(carBookingVM);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RegNr,CarSize,CurrentDistance,Mileage,Rented")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _carsRepo.Update(car);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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

            CarBookingVM carBookingVM = new CarBookingVM();
            carBookingVM.CarSizes = GetCarSizeToList();
            carBookingVM.Car = car;

            return View(carBookingVM);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var car = _carsRepo.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var car = _carsRepo.GetCarById(id);
            _carsRepo.DeleteCar(car);

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(Guid id)
        {
            return _carsRepo.Exists(id);
        }
    }
}

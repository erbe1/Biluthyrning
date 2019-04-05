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

namespace HyrBil.Views
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()//(string sortOrder)
        {
            //ViewBag.RegNrSort = String.IsNullOrEmpty(sortOrder) ? "regDesc" : "";

            //var cars = from c in _context.Cars select c;

            //switch (sortOrder)
            //{
            //    case "regDesc":
            //        cars = cars.OrderByDescending(c => c.RegNr);
            //        break;
            //    default:
            //        cars = cars.OrderBy(c => c.CarSize);
            //        break;
            //}


            return View(await _context.Cars.ToListAsync());
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

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var carsInDb = _context.Cars.FirstOrDefault(x => x.RegNr == car.RegNr);

            //if (ModelState.IsValid)
            //{
            if (carsInDb == null)
            {

                ViewBag.UserMessage = $"Bil tillagd med registreringsnummer {car.RegNr.ToUpper()}";
                ModelState.Clear();
                car.Id = Guid.NewGuid();
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                ViewBag.UserMessageFail = $"Det finns redan en bil med registreringsnummer {car.RegNr.ToUpper()}";
                return RedirectToAction(nameof(Index));

            }
            //}
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
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
                    _context.Update(car);
                    await _context.SaveChangesAsync();
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
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(Guid id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}

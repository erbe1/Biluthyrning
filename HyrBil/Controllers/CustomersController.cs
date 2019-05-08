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
    public class CustomersController : Controller
    {
        private readonly ICustomersRepo _customersRepo;

        public CustomersController(ICustomersRepo customersRepo)
        {
            _customersRepo = customersRepo;
        }

        //private readonly ApplicationDbContext _context;

        //public CustomersController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => _customersRepo.GetCustomers()));

            //return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _customersRepo.GetCustomersById(id);
            //var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            CarBookingVM carBookingVM = new CarBookingVM();

            carBookingVM.Customer = customer;

            var theCustomerBookings = _customersRepo.GetCustomerBookings(id);
            //var theCustomerBookings = _context.Bookings.Where(x => x.Customer.CustomerId == id).Include(y => y.Car).ToList();

            carBookingVM.CustomerBookings = theCustomerBookings;

            return View(carBookingVM);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,PNr")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerId = Guid.NewGuid();
                _customersRepo.Add(customer);
                //_context.Add(customer);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _customersRepo.GetCustomersById(id);
            //var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CustomerId,FirstName,LastName,PNr")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _customersRepo.Update(customer);
                    //_context.Update(customer);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = _customersRepo.GetCustomersById(id);
            //var customer = await _context.Customers
            //    .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            //var customer = await _context.Customers.FindAsync(id);
            //_context.Customers.Remove(customer);
            //await _context.SaveChangesAsync();

            var customer = _customersRepo.GetCustomersById(id);
            _customersRepo.DeleteCustomer(customer);

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(Guid id)
        {
            //return _context.Customers.Any(e => e.CustomerId == id);
            return _customersRepo.Exists(id);
        }
    }
}

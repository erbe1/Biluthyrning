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

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => _customersRepo.GetCustomers()));
        }

        // GET: Customers/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _customersRepo.GetCustomersById(id);

            if (customer == null)
            {
                return NotFound();
            }

            CarBookingVM carBookingVM = new CarBookingVM();

            carBookingVM.Customer = customer;

            var theCustomerBookings = _customersRepo.GetCustomerBookings(id);

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
                await _customersRepo.Add(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _customersRepo.GetCustomersById(id);
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
                   await _customersRepo.Update(customer);
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
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = _customersRepo.GetCustomersById(id);
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

            var customer = _customersRepo.GetCustomersById(id);
            await _customersRepo.DeleteCustomer(customer);

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(Guid id)
        {
            return _customersRepo.Exists(id);
        }
    }
}

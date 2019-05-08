using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyrBil.Data;
using HyrBil.Models;
using Microsoft.EntityFrameworkCore;

namespace HyrBil.Services.Repositories
{
    public class CustomersRepo : ICustomersRepo
    {
        private readonly ApplicationDbContext _context;

        public CustomersRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers.ToList();
        }

        public Customer GetCustomersById(Guid? id)
        {
            return _context.Customers.FirstOrDefault(x => x.CustomerId == id);
        }

        IEnumerable<Booking> ICustomersRepo.GetCustomerBookings(Guid? id)
        {
            return _context.Bookings.Where(x => x.Customer.CustomerId == id).Include(y => y.Car).ToList();
        }

        public async Task Add(Customer customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Customer customer)
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public bool Exists(Guid id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

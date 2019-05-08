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

        public void Add(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }

        public void Update(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public bool Exists(Guid id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

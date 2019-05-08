using HyrBil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyrBil.Services.Repositories
{
    public interface ICustomersRepo
    {
        IEnumerable<Customer> GetCustomers();
        Customer GetCustomersById(Guid? id);
        IEnumerable<Booking> GetCustomerBookings(Guid? id);
        void Add(Customer customer);
        void Update(Customer customer);
        void DeleteCustomer(Customer customer);
        bool Exists(Guid id);
    }
}

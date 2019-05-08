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
        Task Add(Customer customer);
        Task Update(Customer customer);
        Task DeleteCustomer(Customer customer);
        bool Exists(Guid id);
    }
}

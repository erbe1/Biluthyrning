using HyrBil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyrBil.Services.Repositories
{
    public interface IBookingsRepo
    {
        IEnumerable<Booking> GetBookings();
        Booking GetBookingsById(Guid? id);
        IEnumerable<Customer> GetCustomers();
        IEnumerable<Car> GetAllCars();
        Task AddBooking(Booking booking);
        Booking GetCarToReturn(Booking booking);
        Task SaveChanges();
        Booking FinishBooking(Booking booking);
        Task Update(Booking finishBooking);
        Task DeleteBooking(Booking booking);
        bool Exists(Guid id);
    }
}

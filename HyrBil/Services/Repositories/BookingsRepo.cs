﻿using HyrBil.Data;
using HyrBil.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyrBil.Services.Repositories
{
    public class BookingsRepo : IBookingsRepo
    {
        private readonly ApplicationDbContext _context;

        public BookingsRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBooking(Booking booking)
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBooking(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public bool Exists(Guid id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        public Booking FinishBooking(Booking booking)
        {
            var finishBooking = _context.Bookings.Include(x => x.Car).Include(y => y.Customer).SingleAsync(z => z.Id == booking.Id);
            return finishBooking.Result;
        }

        public IEnumerable<Car> GetAllCars()
        {
            return _context.Cars.ToList();
        }

        public IEnumerable<Booking> GetBookings()
        {
            return _context.Bookings.Include(x => x.Car).Include(y => y.Customer).ToList();
        }

        public Booking GetBookingsById(Guid? id)
        {
            return _context.Bookings.FirstOrDefault(x => x.Id == id);
        }

        public Booking GetCarToReturn(Booking booking)
        {
            var carToReturn = _context.Bookings.Include(x => x.Car).Include(z => z.Customer).FirstOrDefaultAsync(y => y.Id == booking.Id);
            return carToReturn.Result;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers.ToList();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Booking finishBooking)
        {
            _context.Update(finishBooking);
            await _context.SaveChangesAsync();
        }
    }
}

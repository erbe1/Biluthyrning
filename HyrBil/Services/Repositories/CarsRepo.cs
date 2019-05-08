using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyrBil.Data;
using HyrBil.Models;

namespace HyrBil.Services.Repositories
{
    public class CarsRepo : ICarsRepo
    {
        private readonly ApplicationDbContext _context;

        public CarsRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Car> GetCars()
        {
            return _context.Cars.ToList();
        }

        public Car GetCarById(Guid? id)
        {
            return _context.Cars.FirstOrDefault(x => x.Id == id);
        }

        public Car GetCarsByRegNr(string regNr)
        {
            return _context.Cars.FirstOrDefault(x => x.RegNr == regNr);
        }

        public async Task CreateCar(Car car)
        {
            _context.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCar(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }

        public bool Exists(Guid id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        public async Task Update(Car car)
        {
            _context.Update(car);
            await _context.SaveChangesAsync();
        }
    }
}

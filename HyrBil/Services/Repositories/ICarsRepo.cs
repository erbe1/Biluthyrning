using HyrBil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyrBil.Services.Repositories
{
    public interface ICarsRepo
    {
        IEnumerable<Car> GetCars();
        Car GetCarById(Guid? id);
        Car GetCarsByRegNr(string regNr);
        Task CreateCar(Car car);
        Task DeleteCar(Car car);
        bool Exists(Guid id);
        Task Update(Car car);
    }
}

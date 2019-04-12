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
        void CreateCar(Car car);
        void DeleteCar(Car car);
        bool Exists(Guid id);
        void Update(Car car);
    }
}

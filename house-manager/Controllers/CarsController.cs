using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationContext _context;

        public CarsController(ApplicationContext context)
        {
            _context = context;
        }

        public JsonResult Get()
        {
            var cars = _context.Cars.ToList();
            return Json(cars);
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            var car = _context.Cars.FirstOrDefault(l => l.Id == int.Parse(id));
            return Json(car);
        }

        [HttpPost]
        public JsonResult Update(Car car)
        {
            if (ModelState.IsValid && !_context.Cars.Any(x => x.RegistrationNumber == car.RegistrationNumber))
            {
                _context.Cars.Update(car);
                _context.SaveChanges();
                return Json("Car updated successfully");
            }
            if (ModelState["RegistrationNumber"].Errors.Count > 0)
                return Json(new { success = false, message = ModelState["RegistrationNumber"].Errors[0] });
            return Json(new { success = false, message = "Машина с таким номером уже добавлена" });
        }

        [HttpPost]
        public JsonResult Insert(Car car, string id)
        {

            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                _context.SaveChanges();
                _context.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = int.Parse(id) });
                _context.SaveChanges();
                return Json(new { success = true, message = "Car added successfully" });
            }
            return Json(new { success = false, message = ModelState["car.RegistrationNumber"].Errors[0] });
        }

        [HttpPost]
        public JsonResult Delete(string id, string ownerId)
        {
            var car = _context.Cars.Find(int.Parse(id));
            if (car != null && !_context.OwnedCars.Any(x => x.CarId == car.Id && x.OwnerId != int.Parse(ownerId)))
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();
                return Json(new { success = true, message = "Car deleted successfully" });
            }
            else
            {
                var lodger = _context.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(ownerId));
                if (lodger != null)
                {
                    _context.OwnedCars.Remove(_context.OwnedCars.FirstOrDefault(x => x.CarId == car.Id && x.OwnerId == lodger.Id));
                    _context.SaveChanges();
                }
            }
            return Json(new { success = false, message = "Car not found" });
        }

        [HttpPost]
        public JsonResult UpdateCars(string id, List<OwnedCar> selectedCars)
        {
            var lodger = _context.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(id));
            if (lodger != null)
            {
                var lodgerCars = _context.OwnedCars.Where(x => x.OwnerId == lodger.Id);
                foreach (var car in lodgerCars)
                {
                    _context.OwnedCars.Remove(car);
                }
                _context.SaveChanges();

                foreach (var item in selectedCars)
                {
                    var car = _context.Cars.Find(item.Id);
                    if (car != null)
                    {
                        _context.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = lodger.Id });
                    }
                }

                _context.SaveChanges();
                return Json("Выбранные машины успешно добавлены");
            }
            return Json("Ошибка при добавлении выбранных машин");
        }
    }
}

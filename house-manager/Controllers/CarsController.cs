using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class CarsController : Controller
    {
        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var cars = dataBase.Cars.ToList();
                    return Json(cars);
                }
                return Json(null);
            }
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var car = dataBase.Cars.FirstOrDefault(l => l.Id == int.Parse(id));
                    return Json(car);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Update(Car car)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    if (dataBase.Cars.Any(l => l.RegistrationNumber == car.RegistrationNumber))
                        return Json(new { success = false, message = "Автомобиль с таким регистрационным номером уже существует" });
                    if (ModelState.IsValid && !dataBase.Cars.Any(x => x.RegistrationNumber == car.RegistrationNumber))
                    {
                        dataBase.Cars.Update(car);
                        dataBase.SaveChanges();
                        return Json("Car updated successfully");
                    }
                    if (ModelState["RegistrationNumber"].Errors.Count > 0)
                        return Json(new { success = false, message = ModelState["RegistrationNumber"].Errors[0] });
                    return Json(new { success = false, message = "Машина с таким номером уже добавлена" });
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Insert(Car car, string id)
        {
            if (ModelState.IsValid)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                    if (dataBase != null)
                    {
                        dataBase.Cars.Add(car);
                        dataBase.SaveChanges();
                        dataBase.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = int.Parse(id) });
                        dataBase.SaveChanges();
                        return Json(new { success = true, message = "Car added successfully" });
                    }
                    return Json(null);
                }
            }
            return Json(new { success = false, message = ModelState["car.RegistrationNumber"].Errors[0] });
        }

        [HttpPost]
        public JsonResult Delete(string id, string ownerId)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var car = dataBase.Cars.Find(int.Parse(id));
                    if (car != null && !dataBase.OwnedCars.Any(x => x.CarId == car.Id && x.OwnerId != int.Parse(ownerId)))
                    {
                        dataBase.Cars.Remove(car);
                        dataBase.SaveChanges();
                        return Json(new { success = true, message = "Car deleted successfully" });
                    }
                    else
                    {
                        var lodger = dataBase.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(ownerId));
                        if (lodger != null)
                        {
                            dataBase.OwnedCars.Remove(dataBase.OwnedCars.FirstOrDefault(x => x.CarId == car.Id && x.OwnerId == lodger.Id));
                            dataBase.SaveChanges();
                        }
                    }
                    return Json(new { success = false, message = "Car not found" });
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult UpdateCars(string id, List<OwnedCar> selectedCars)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodger = dataBase.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(id));
                    if (lodger != null)
                    {
                        var lodgerCars = dataBase.OwnedCars.Where(x => x.OwnerId == lodger.Id);
                        foreach (var car in lodgerCars)
                        {
                            dataBase.OwnedCars.Remove(car);
                        }
                        dataBase.SaveChanges();

                        foreach (var item in selectedCars)
                        {
                            var car = dataBase.Cars.Find(item.Id);
                            if (car != null)
                            {
                                dataBase.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = lodger.Id });
                            }
                        }

                        dataBase.SaveChanges();
                        return Json("Выбранные машины успешно добавлены");
                    }
                    return Json("Ошибка при добавлении выбранных машин");
                }
                return Json(null);
            }
        }
    }
}

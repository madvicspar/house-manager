using house_manager.Models;
using house_manager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class CarsController : Controller
    {
        /// <summary>
        /// Retrieves a list of all cars from the database
        /// </summary>
        /// <returns></returns>
        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                return Json(dataBase.Cars.ToList());
            }
        }

        /// <summary>
        /// Retrieves the details of an car for editing
        /// </summary>
        /// <param name="id">The ID of the owned car to be edited</param>
        /// <returns>JSON object containing the details of the owned car</returns>
        [HttpGet]
        public JsonResult Edit(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                return Json(dataBase.Cars.FirstOrDefault(l => l.Id == int.Parse(id)));
            }
        }

        /// <summary>
        /// Updates the specific owned car
        /// </summary>
        /// <param name="car">The specific owned car</param>
        /// <returns>JSON object indicating the success or failure of the update operation and message</returns>
        [HttpPost]
        public JsonResult Update(Car car)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                if (dataBase.Cars.Any(l => l.RegistrationNumber == car.RegistrationNumber))
                {
                    return Json(new { success = false, message = "Автомобиль с таким регистрационным номером уже существует" });
                }

                if (ModelState.IsValid && !dataBase.Cars.Any(x => x.RegistrationNumber == car.RegistrationNumber))
                {
                    dataBase.Cars.Update(car);
                    dataBase.SaveChanges();
                    return Json("Car updated successfully");
                }
                if (ModelState["RegistrationNumber"]?.Errors.Count > 0)
                {
                    return Json(new { success = false, message = ModelState["RegistrationNumber"].Errors[0] });
                }
                return Json(new { success = false, message = "Машина с таким номером уже добавлена" });
            }
        }

        /// <summary>
        /// Inserts a new car into the database and assigns it to a lodger
        /// </summary>
        /// <param name="car">The car object to be inserted</param>
        /// <param name="id">The ID of the owner to associate the car with</param>
        /// <returns>JSON object indicating the success or failure of the insert operation and message</returns>
        [HttpPost]
        public JsonResult Insert(Car car, string id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = ModelState["car.RegistrationNumber"].Errors[0] });
            }

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                dataBase.Cars.Add(car);
                dataBase.SaveChanges();
                dataBase.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = int.Parse(id) });
                dataBase.SaveChanges();
                return Json(new { success = true, message = "Car added successfully" });
            }
        }

        /// <summary>
        /// Deletes the specific owned car
        /// </summary>
        /// <param name="id">The ID of the car to be removed</param>
        /// <param name="ownerId">The ID of the car's owner</param>
        /// <returns>JSON object indicating the success or failure of the delete operation and message</returns>
        [HttpPost]
        public JsonResult Delete(string id, string ownerId)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                var car = dataBase.Cars.Find(int.Parse(id));
                if (car != null && !dataBase.OwnedCars.Any(x => x.CarId == car.Id && x.OwnerId != int.Parse(ownerId)))
                {
                    dataBase.Cars.Remove(car);
                    dataBase.SaveChanges();
                    return Json(new { success = true, message = "Car deleted successfully" });
                }

                var lodger = dataBase.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(ownerId));
                if (lodger != null)
                {
                    dataBase.OwnedCars.Remove(dataBase.OwnedCars.FirstOrDefault(x => x.CarId == car.Id && x.OwnerId == lodger.Id));
                    dataBase.SaveChanges();
                }
                return Json(new { success = false, message = "Car not found" });
                
            }
        }

        /// <summary>
        /// Updates the list of owned cars for a specific lodger
        /// </summary>
        /// <param name="id">The ID of the lodger</param>
        /// <param name="selectedCars">New list of cars owned for the lodger</param>
        /// <returns>JSON object indicating the success or failure of the update operation</returns>
        [HttpPost]
        public JsonResult UpdateCars(string id, List<OwnedCar> selectedCars)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                var lodger = dataBase.Lodgers.Include(l => l.OwnedCars).FirstOrDefault(l => l.Id == int.Parse(id));
                if (lodger == null)
                {
                    return Json("Ошибка при добавлении выбранных машин");
                }

                var lodgerCars = dataBase.OwnedCars.Where(x => x.OwnerId == lodger.Id);
                dataBase.OwnedCars.RemoveRange(lodgerCars);
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
        }
    }
}

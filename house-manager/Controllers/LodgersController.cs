using house_manager.Models;
using house_manager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class LodgersController : Controller
    {
        // GET: Lodgers
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieves a list of all lodgers from the database
        /// </summary>
        /// <returns>JSON object representing the list of lodgers or NULL</returns>
        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                return Json(dataBase.Lodgers
                    .Include(c => c.OwnedCars).ThenInclude(c => c.Car)
                    .Include(x => x.OwnedApartments).ThenInclude(ap => ap.Apartment)
                    .Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace)
                    .OrderBy(x => x.Surname + x.Name + x.Pathronymic)
                    .ToList());
            }
        }

        /// <summary>
        /// Retrieves the address of the house
        /// </summary>
        /// <returns>JSON object representing the house's address</returns>
        public JsonResult GetHouseAddress()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                return dataBase == null ? Json(null) : Json(dataBase.Houses.First().Address);
            }
        }

        /// <summary>
        /// Inserts a new lodger into the database
        /// </summary>
        /// <param name="lodger">The lodger object to be inserted</param>
        /// <returns>JSON object indicating the success or failure of the insert operation and message</returns>
        [HttpPost]
        public JsonResult Insert(Lodger lodger)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model validation failed" });
            }

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                if (dataBase.Lodgers.Any(l => l.PassportNumber == lodger.PassportNumber))
                {
                    return Json(new { success = false, message = "Жилец с такими паспортными данными уже существует" });
                }

                if (!dataBase.Lodgers.Any(l => l.Name == lodger.Name 
                        && l.Surname == lodger.Surname
                        && l.Pathronymic == lodger.Pathronymic
                        && l.PassportNumber == lodger.PassportNumber))
                {
                    dataBase.Lodgers.Add(lodger);
                    dataBase.SaveChanges();
                    return Json(new { success = true, message = "Lodger added successfully" });
                }
                return Json(new { success = false, message = "Lodger already exists" });
            }
        }

        /// <summary>
        /// Retrieves the details of an lodger for editing
        /// </summary>
        /// <param name="id">The ID of the lodger to be edited</param>
        /// <returns>JSON object containing the details of the lodger</returns>
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

                return Json(dataBase.Lodgers.Include(c => c.OwnedCars).ThenInclude(c => c.Car)
                    .Include(l => l.OwnedApartments).ThenInclude(ap => ap.Apartment).Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace)
                    .FirstOrDefault(l => l.Id == int.Parse(id)));
            }
        }

        /// <summary>
        /// Updates the specific lodger
        /// </summary>
        /// <param name="lodger">The specific lodger</param>
        /// <returns>JSON object indicating the success or failure of the update operation and message</returns>
        [HttpPost]
        public JsonResult Update(Lodger lodger)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = ModelState["PassportNumber"].Errors[0] });
            }

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                if (dataBase.Lodgers.Any(l => l.PassportNumber == lodger.PassportNumber))
                {
                    return Json(new { success = false, message = "Жилец с такими паспортными данными уже существует" });
                }

                if (!dataBase.Lodgers.Any(l => l.Name == lodger.Name 
                    && l.Surname == lodger.Surname
                    && l.Pathronymic == lodger.Pathronymic
                    && l.PassportNumber == lodger.PassportNumber))
                {
                    dataBase.Lodgers.Update(lodger);
                    dataBase.SaveChanges();
                    return Json(new { success = true, message = "Lodger updated successfully" });
                }
                return Json(new { success = false, message = "Lodger with such data already exists" });
            }
        }

        /// <summary>
        /// Deletes the specific lodger
        /// </summary>
        /// <param name="id">The ID of the specific lodger</param>
        /// <returns>JSON object indicating the success or failure of the delete operation and message</returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                var lodger = dataBase.Lodgers.Find(int.Parse(id));
                if (lodger == null)
                {
                    return Json("Lodger not found");
                }

                dataBase.Lodgers.Remove(lodger);
                dataBase.SaveChanges();
                dataBase.Entry(lodger).State = EntityState.Detached;
                return Json("Lodger deleted successfully");
            }
        }

        /// <summary>
        /// Searches for lodgers based on the provided criteria
        /// </summary>
        /// <param name="surname">Lodger's surname</param>
        /// <param name="name">Lodger's name</param>
        /// <param name="pathronymic">Lodger's pathronymic</param>
        /// <param name="passportNumber">Lodger's passportNumber</param>
        /// <param name="apartmentNumber">Apartment number associated with the lodger</param>
        /// <param name="registrationNumber">Car registration number associated with the lodger</param>
        /// <param name="brand">Car brand associated with the lodger</param>
        /// <param name="parkingSpaceNumber">Parking space number associated with the lodger</param>
        /// <returns>JSON with a list of lodgers matching the search criteria</returns>
        public JsonResult Search(string surname, string name, string pathronymic, string passportNumber, string apartmentNumber, string registrationNumber, string brand, string parkingSpaceNumber)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                var lodgers = dataBase.Lodgers.Include(x => x.OwnedParkingSpaces).ThenInclude(p => p.ParkingSpace).Include(x => x.OwnedApartments).ThenInclude(p => p.Apartment).Include(x => x.OwnedCars).ThenInclude(p => p.Car)
                    .Where(l =>
                        (string.IsNullOrEmpty(surname) || l.Surname.Contains(surname)) &&
                        (string.IsNullOrEmpty(name) || l.Name.Contains(name)) &&
                        (string.IsNullOrEmpty(pathronymic) || l.Pathronymic.Contains(pathronymic)) &&
                        (string.IsNullOrEmpty(passportNumber) || l.PassportNumber.Contains(passportNumber)) &&
                        (string.IsNullOrEmpty(apartmentNumber) || l.OwnedApartments.Any(p => p.Apartment != null && p.Apartment.Number.Contains(apartmentNumber))) &&
                        (string.IsNullOrEmpty(registrationNumber) || l.OwnedCars.Any(p => p.Car != null && p.Car.RegistrationNumber.Contains(registrationNumber))) &&
                        (string.IsNullOrEmpty(brand) || l.OwnedCars.Any(p => p.Car != null && p.Car.Brand.Contains(brand))) &&
                        (string.IsNullOrEmpty(parkingSpaceNumber) || l.OwnedParkingSpaces.Any(p => p.ParkingSpace != null && p.ParkingSpace.Number.Contains(parkingSpaceNumber)))
                ).OrderBy(x => x.Surname + x.Name + x.Pathronymic).ToList();

                return Json(lodgers);
            }
        }
    }
}
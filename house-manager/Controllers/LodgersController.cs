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

        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodgers = dataBase.Lodgers.Include(c => c.OwnedCars).ThenInclude(c => c.Car).Include(x => x.OwnedApartments).ThenInclude(ap => ap.Apartment).Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace).OrderBy(x => x.Surname + x.Name + x.Pathronymic).ToList();
                    return Json(lodgers);
                }
                return Json(null);
            }
        }

        public JsonResult GetHouseAdress()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    return Json(dataBase.Houses.First().Address);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Insert(Lodger lodger)
        {
            if (ModelState.IsValid)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                    if (dataBase != null)
                    {
                        if (dataBase.Lodgers.Any(l => l.PassportNumber == lodger.PassportNumber))
                            return Json(new { success = false, message = "Жилец с такими паспортными данными уже существует" });
                        if (!dataBase.Lodgers.Any(l => l.Name == lodger.Name && l.Surname == lodger.Surname && l.Pathronymic == lodger.Pathronymic && l.PassportNumber == lodger.PassportNumber))
                        {
                            dataBase.Lodgers.Add(lodger);
                            dataBase.SaveChanges();
                            return Json(new { success = true, message = "Lodger added successfully" });
                        }
                        return Json(new { success = false, message = "Lodger already exists" });
                    }
                    return Json(new { success = false, message = "Model validation failed" });
                }
            }
            return Json(new { success = false, message = "Model validation failed" });
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodger = dataBase.Lodgers.Include(c => c.OwnedCars).ThenInclude(c => c.Car)
                        .Include(l => l.OwnedApartments).ThenInclude(ap => ap.Apartment).Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace)
                        .FirstOrDefault(l => l.Id == int.Parse(id));
                    return Json(lodger);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Update(Lodger lodger)
        {
            if (ModelState.IsValid)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                    if (dataBase != null)
                    {
                        if (dataBase.Lodgers.Any(l => l.PassportNumber == lodger.PassportNumber))
                            return Json(new { success = false, message = "Жилец с такими паспортными данными уже существует" });
                        if (!dataBase.Lodgers.Any(l => l.Name == lodger.Name && l.Surname == lodger.Surname && l.Pathronymic == lodger.Pathronymic && l.PassportNumber == lodger.PassportNumber))
                        {
                            dataBase.Lodgers.Update(lodger);
                            dataBase.SaveChanges();
                            return Json(new { success = true, message = "Lodger updated successfully" });
                        }
                        return Json(new { success = false, message = "Lodger with such data already exists" });
                    }
                    return Json(null);
                }
            }

            return Json(new { success = false, message = ModelState["PassportNumber"].Errors[0] });
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodger = dataBase.Lodgers.Find(int.Parse(id));
                    if (lodger != null)
                    {
                        dataBase.Lodgers.Remove(lodger);
                        dataBase.SaveChanges();
                        dataBase.Entry(lodger).State = EntityState.Detached;
                        return Json("Lodger deleted successfully");
                    }
                    return Json("Lodger not found");
                }
                return Json(null);
            }
        }

        public JsonResult Search(string surname, string name, string pathronymic, string passportNumber, string apartmentNumber, string registrationNumber, string brand, string parkingSpaceNumber)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
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
                return Json(null);
            }
        }
    }
}
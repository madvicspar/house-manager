using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class LodgersController : Controller
    {
        private readonly ApplicationContext _context;

        public LodgersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Lodgers
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var lodgers = _context.Lodgers.Include(c => c.OwnedCars).ThenInclude(c => c.Car).Include(x => x.OwnedApartments).ThenInclude(ap => ap.Apartment).Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace).OrderBy(x => x.Surname + x.Name + x.Pathronymic).ToList();
            return Json(lodgers);
        }

        public JsonResult GetHouseAdress()
        {
            return Json(_context.Houses.First().Address);
        }

        [HttpPost]
        public JsonResult Insert(Lodger lodger)
        {
            if (ModelState.IsValid)
            {
                _context.Lodgers.Add(lodger);
                _context.SaveChanges();
                return Json("Lodger added successfully");
            }
            return Json("Model validation failed");
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            var lodger = _context.Lodgers.Include(c => c.OwnedCars).ThenInclude(c => c.Car)
                .Include(l => l.OwnedApartments).ThenInclude(ap => ap.Apartment).Include(ps => ps.OwnedParkingSpaces).ThenInclude(ap => ap.ParkingSpace)
                .FirstOrDefault(l => l.Id == int.Parse(id));
            return Json(lodger);
        }

        [HttpPost]
        public JsonResult Update(Lodger lodger)
        {
            if (ModelState.IsValid)
            {
                _context.Lodgers.Update(lodger);
                _context.SaveChanges();
                return Json(new { success = true, message = "Lodger updated successfully" });
            }

            return Json(new { success = false, message = ModelState["PassportNumber"].Errors[0] });
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            var lodger = _context.Lodgers.Find(int.Parse(id));
            if (lodger != null)
            {
                _context.Lodgers.Remove(lodger);
                _context.SaveChanges();
                return Json("Lodger deleted successfully");
            }
            return Json("Lodger not found");
        }

        public JsonResult Search(string surname, string name, string pathronymic, string passportNumber, string apartmentNumber, string registrationNumber, string brand, string parkingSpaceNumber)
        {
            var lodgers = _context.Lodgers.Include(x => x.OwnedParkingSpaces).ThenInclude(p => p.ParkingSpace).Include(x => x.OwnedApartments).ThenInclude(p => p.Apartment).Include(x => x.OwnedCars).ThenInclude(p => p.Car)
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
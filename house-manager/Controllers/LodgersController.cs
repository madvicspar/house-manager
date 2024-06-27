using Azure;
using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public JsonResult GetLodgers()
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
                return Json(new { success = false, message = "Lodger updated successfully" } );
            }
            
            return Json( new { success = false, message = ModelState["PassportNumber"].Errors[0] });
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

        [HttpGet]
        public JsonResult EditCar(string id)
        {
            var car = _context.Cars.FirstOrDefault(l => l.Id == int.Parse(id));
            return Json(car);
        }

        [HttpPost]
        public JsonResult UpdateCar(Car car)
        {
            if (ModelState.IsValid && !_context.Cars.Any(x => x.RegistrationNumber == car.RegistrationNumber))
            {
                _context.Cars.Update(car);
                _context.SaveChanges();
                return Json("Car updated successfully");
            }
            return Json(new { success = false, message = "Машина с таким номером уже добавлена" });
        }

        [HttpPost]
        public JsonResult InsertCar(Car car, string id)
        {
            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                _context.SaveChanges();
                _context.OwnedCars.Add(new OwnedCar { CarId = car.Id, OwnerId = int.Parse(id) });
                _context.SaveChanges();
                return Json("Car added successfully");
            }
            return Json("Model validation failed");
        }

        [HttpPost]
        public JsonResult DeleteCar(string id, string ownerId)
        {
            var car = _context.Cars.Find(int.Parse(id));
            if (car != null && !_context.OwnedCars.Any(x => x.CarId == car.Id && x.OwnerId != int.Parse(ownerId)))
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();
                return Json(new { success = true, message = "Car deleted successfully" } );
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
            return Json(new { success = false, message = "Car not found" } );
        }

        public JsonResult GetAllApartments()
        {
            var apartments = _context.Apartments.ToList();
            return Json(apartments);
        }

        [HttpPost]
        public JsonResult UpdateApartments(string id, List<Apartment> selectedApartments)
        {
            var lodger = _context.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(id));
            if (lodger != null)
            {
                var lodgerApartments = _context.OwnedApartments.Where(x => x.OwnerId == lodger.Id);
                foreach (var apartment in lodgerApartments)
                {
                    _context.OwnedApartments.Remove(apartment);
                }
                _context.SaveChanges();

                // Создаем новые связи для выбранных квартир
                foreach (var item in selectedApartments)
                {
                    var apartment = _context.Apartments.Find(item.Id);
                    if (apartment != null)
                    {
                        _context.OwnedApartments.Add(new OwnedApartment { ApartmentId = apartment.Id, OwnerId = lodger.Id });
                        _context.SaveChanges();
                        apartment.ResidentsNumber++;
                        _context.Apartments.Update(apartment);
                        _context.SaveChanges();
                    }
                }

                _context.SaveChanges();
                return Json("Выбранные квартиры успешно добавлены");
            }
            return Json("Ошибка при добавлении выбранных квартир");
        }

        public JsonResult GetAllCars()
        {
            var cars = _context.Cars.ToList();
            return Json(cars);
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

                // Создаем новые связи для выбранных квартир
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

        public JsonResult GetAllParkingSpaces(string id)
        {
            var parkingSpaces = _context.ParkingSpaces.Where(x => !_context.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id) || _context.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id && ops.OwnerId == int.Parse(id))).ToList();
            return Json(parkingSpaces);
        }

        [HttpPost]
        public JsonResult UpdateParkingSpaces(string id, List<ParkingSpace> selectedParkingSpaces)
        {
            var lodger = _context.Lodgers.Include(l => l.OwnedParkingSpaces).FirstOrDefault(l => l.Id == int.Parse(id));
            if (lodger != null)
            {
                var lodgerOwnedParkingSpaces = _context.OwnedParkingSpaces.Where(x => x.OwnerId == lodger.Id);
                foreach (var parkingSpace in lodgerOwnedParkingSpaces)
                {
                    _context.OwnedParkingSpaces.Remove(parkingSpace);
                }
                _context.SaveChanges();

                foreach (var item in selectedParkingSpaces)
                {
                    var parkingSpace = _context.ParkingSpaces.Find(item.Id);
                    if (parkingSpace != null)
                    {
                        _context.OwnedParkingSpaces.Add(new OwnedParkingSpace() { ParkingSpaceId = parkingSpace.Id, OwnerId = lodger.Id });
                    }
                }

                _context.SaveChanges();
                return Json("Выбранные парковочные места успешно забронированы");
            }
            return Json("Ошибка при бронировании выбранных парковочных мест");
        }

        [HttpPost]
        public JsonResult DeleteParkingSpace(string id)
        {
            var parkingSpace = _context.OwnedParkingSpaces.FirstOrDefault(x => x.ParkingSpaceId == int.Parse(id));
            if (parkingSpace != null)
            {
                _context.OwnedParkingSpaces.Remove(parkingSpace);
                _context.SaveChanges();
                return Json("ParkingSpace deleted successfully");
            }
            return Json("ParkingSpace not found");
        }

        [HttpGet]
        public JsonResult EditOwnedApartment(string id)
        {
            var ownedApartment = _context.OwnedApartments.Where(l => l.Id == int.Parse(id)).Include(x => x.Apartment).First();
            return Json(ownedApartment);
        }

        [HttpPost]
        public JsonResult UpdateOwnedApartment(string id, string ownershipPercentage, string ownerId)
        {
            if (!float.TryParse(ownershipPercentage, out float temp))
                return Json(new { success = false, message = "Вводите только числа" });
            var ownedApartment = _context.OwnedApartments.Find(int.Parse(id));
            var sum = _context.OwnedApartments.Where(x => x.ApartmentId == ownedApartment.ApartmentId && x.OwnerId != int.Parse(ownerId)).Sum(x => x.OwnershipPercentage);
            if (sum + float.Parse(ownershipPercentage) > 100)
                return Json(new { success = false, message = $"Ваша доля стоимости не должна превышать {(100 - sum)}%" });
            ownedApartment.OwnershipPercentage = float.Parse(ownershipPercentage);
            _context.OwnedApartments.Update(ownedApartment);
            _context.SaveChanges();
            return Json( new { success = true, message = "Owned Apartment updated successfully" } );
        }
    }
}
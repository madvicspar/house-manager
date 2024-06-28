using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly ApplicationContext _context;

        public ApartmentsController(ApplicationContext context)
        {
            _context = context;
        }

        public JsonResult Get()
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
                var ownedApartmentsIds = lodger.OwnedApartments.Where(x => selectedApartments.Any(c => c.Id == x.ApartmentId)).Select(x => x.ApartmentId).ToList();

                var lodgerApartments = _context.OwnedApartments.Where(x => x.OwnerId == lodger.Id);
                foreach (var apartment in lodgerApartments)
                {
                    if (!ownedApartmentsIds.Contains(apartment.ApartmentId))
                        _context.OwnedApartments.Remove(apartment);
                }
                _context.SaveChanges();

                foreach (var item in selectedApartments)
                {
                    if (ownedApartmentsIds.Contains(item.Id))
                        continue;
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

        [HttpGet]
        public JsonResult Edit(string id)
        {
            var ownedApartment = _context.OwnedApartments.Where(l => l.Id == int.Parse(id)).Include(x => x.Apartment).First();
            return Json(ownedApartment);
        }

        [HttpPost]
        public JsonResult Update(string id, string ownershipPercentage, string ownerId)
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
            return Json(new { success = true, message = "Owned Apartment updated successfully" });
        }

        [HttpPost]
        public JsonResult Delete(string id, string ownerId)
        {
            var apartment = _context.Apartments.Find(int.Parse(id));
            var lodger = _context.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(ownerId));
            if (lodger != null)
            {
                _context.OwnedApartments.Remove(_context.OwnedApartments.FirstOrDefault(x => x.ApartmentId == apartment.Id && x.OwnerId == lodger.Id));
                _context.SaveChanges();
                apartment.ResidentsNumber--;
                _context.Apartments.Update(apartment);
                _context.SaveChanges();
            }
            return Json(new { success = false, message = "Apartment not found" });
        }
    }
}

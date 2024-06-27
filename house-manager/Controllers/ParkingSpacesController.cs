using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ParkingSpacesController : Controller
    {
        private readonly ApplicationContext _context;

        public ParkingSpacesController(ApplicationContext context)
        {
            _context = context;
        }

        public JsonResult Get(string id)
        {
            var parkingSpaces = _context.ParkingSpaces.Where(x => !_context.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id) || _context.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id && ops.OwnerId == int.Parse(id))).ToList();
            return Json(parkingSpaces);
        }

        [HttpPost]
        public JsonResult Update(string id, List<ParkingSpace> selectedParkingSpaces)
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
        public JsonResult Delete(string id)
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
    }
}

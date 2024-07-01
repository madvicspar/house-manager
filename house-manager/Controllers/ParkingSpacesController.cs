using house_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ParkingSpacesController : Controller
    {
        public JsonResult Get(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var parkingSpaces = dataBase.ParkingSpaces.Where(x => !dataBase.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id) || dataBase.OwnedParkingSpaces.Any(ops => ops.ParkingSpaceId == x.Id && ops.OwnerId == int.Parse(id))).ToList();
                    return Json(parkingSpaces);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Update(string id, List<ParkingSpace> selectedParkingSpaces)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodger = dataBase.Lodgers.Include(l => l.OwnedParkingSpaces).FirstOrDefault(l => l.Id == int.Parse(id));
                    if (lodger != null)
                    {
                        var lodgerOwnedParkingSpaces = dataBase.OwnedParkingSpaces.Where(x => x.OwnerId == lodger.Id);
                        foreach (var parkingSpace in lodgerOwnedParkingSpaces)
                        {
                            dataBase.OwnedParkingSpaces.Remove(parkingSpace);
                        }
                        dataBase.SaveChanges();

                        foreach (var item in selectedParkingSpaces)
                        {
                            var parkingSpace = dataBase.ParkingSpaces.Find(item.Id);
                            if (parkingSpace != null)
                            {
                                dataBase.OwnedParkingSpaces.Add(new OwnedParkingSpace() { ParkingSpaceId = parkingSpace.Id, OwnerId = lodger.Id });
                            }
                        }

                        dataBase.SaveChanges();
                        return Json("Выбранные парковочные места успешно забронированы");
                    }
                    return Json("Ошибка при бронировании выбранных парковочных мест");
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var parkingSpace = dataBase.OwnedParkingSpaces.FirstOrDefault(x => x.ParkingSpaceId == int.Parse(id));
                    if (parkingSpace != null)
                    {
                        dataBase.OwnedParkingSpaces.Remove(parkingSpace);
                        dataBase.SaveChanges();
                        return Json("ParkingSpace deleted successfully");
                    }
                    return Json("ParkingSpace not found");
                }
                return Json(null);
            }
        }
    }
}

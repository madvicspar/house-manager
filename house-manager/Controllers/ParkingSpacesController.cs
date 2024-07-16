using house_manager.Models;
using house_manager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ParkingSpacesController : Controller
    {
        /// <summary>
        /// Retrieves a list of owned parking spaces from the database
        /// </summary>
        /// <param name="id">The ID of the parking spaces' owner</param>
        /// <returns>JSON object representing the list of owned parking spaces or NULL</returns>
        public JsonResult Get(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }
                return Json(dataBase.ParkingSpaces
                    .Where(x => !dataBase.OwnedParkingSpaces
                        .Any(ops => ops.ParkingSpaceId == x.Id) || dataBase.OwnedParkingSpaces
                            .Any(ops => ops.ParkingSpaceId == x.Id && ops.OwnerId == int.Parse(id))).ToList());
            }
        }

        /// <summary>
        /// Updates the list of owned apartments for a specific lodger
        /// </summary>
        /// <param name="id">The ID of the parking spaces' owner</param>
        /// <param name="selectedParkingSpaces">New list of parking spaces owned for the lodger</param>
        /// <returns>JSON object indicating the success or failure of the update operation and message</returns>
        [HttpPost]
        public JsonResult Update(string id, List<ParkingSpace> selectedParkingSpaces)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }
                var lodger = dataBase.Lodgers.Include(l => l.OwnedParkingSpaces).FirstOrDefault(l => l.Id == int.Parse(id));
                if (lodger == null)
                {
                    return Json("Ошибка при бронировании выбранных парковочных мест");
                }

                var lodgerOwnedParkingSpaces = dataBase.OwnedParkingSpaces.Where(x => x.OwnerId == lodger.Id);
                dataBase.OwnedParkingSpaces.RemoveRange(lodgerOwnedParkingSpaces);
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
        }

        /// <summary>
        /// Deletes a parking space from the user's owned parking spaces
        /// </summary>
        /// <param name="id">The ID of the parking space to be deleted</param>
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
                    var parkingSpace = dataBase.OwnedParkingSpaces.FirstOrDefault(x => x.ParkingSpaceId == int.Parse(id));
                if (parkingSpace == null)
                {
                    return Json("ParkingSpace not found");
                }

                dataBase.OwnedParkingSpaces.Remove(parkingSpace);
                dataBase.SaveChanges();
                return Json("ParkingSpace deleted successfully");
            }
        }
    }
}

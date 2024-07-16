using house_manager.Models;
using house_manager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ApartmentsController : Controller
    {
        /// <summary>
        /// Retrieves a list of all apartments from the database
        /// </summary>
        /// <returns>JSON object representing the list of apartments or NULL</returns>
        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }
                return Json(dataBase.Apartments.ToList());
            }
        }

        /// <summary>
        /// Retrieves the details of an owned apartment for editing
        /// </summary>
        /// <param name="id">The ID of the owned apartment to be edited</param>
        /// <returns>JSON object containing the details of the owned apartment</returns>
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

                var ownedApartment = dataBase.OwnedApartments.Where(l => l.Id == int.Parse(id)).Include(x => x.Apartment).First();
                return Json(ownedApartment);
            }
        }

        /// <summary>
        /// Updates the specific owned apartment
        /// </summary>
        /// <param name="id">The ID of the owned apartment to be edited</param>
        /// <param name="ownershipPercentage">Lodger's percentage of ownership in the apartment</param>
        /// <param name="ownerId">The ID of the apartment's owner</param>
        /// <returns>JSON object indicating the success or failure of the update operation and message</returns>
        [HttpPost]
        public JsonResult Update(string id, string ownershipPercentage, string ownerId)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                if (!float.TryParse(ownershipPercentage, out float temp))
                {
                    return Json(new { success = false, message = "Вводите только числа" });
                }

                var ownedApartment = dataBase.OwnedApartments.Find(int.Parse(id));
                var sum = dataBase.OwnedApartments.Where(x => x.ApartmentId == ownedApartment.ApartmentId && x.OwnerId != int.Parse(ownerId)).Sum(x => x.OwnershipPercentage);
                if (sum + float.Parse(ownershipPercentage) > 100)
                {
                    return Json(new { success = false, message = $"Ваша доля стоимости не должна превышать {(100 - sum)}%" });
                }

                ownedApartment.OwnershipPercentage = float.Parse(ownershipPercentage);
                dataBase.OwnedApartments.Update(ownedApartment);
                dataBase.SaveChanges();
                return Json(new { success = true, message = "Owned Apartment updated successfully" });
            }
        }

        /// <summary>
        /// Deletes the specific owned apartment
        /// </summary>
        /// <param name="id">The ID of the owned apartment to be removed</param>
        /// <param name="ownerId">The ID of the apartment's owner</param>
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

                var apartment = dataBase.Apartments.Find(int.Parse(id));
                var lodger = dataBase.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(ownerId));
                if (lodger == null)
                {
                    return Json(new { success = false, message = "Lodger not found" });
                }

                dataBase.OwnedApartments.Remove(dataBase.OwnedApartments.FirstOrDefault(x => x.ApartmentId == apartment.Id && x.OwnerId == lodger.Id));
                dataBase.SaveChanges();
                apartment.ResidentsNumber--;
                dataBase.Apartments.Update(apartment);
                dataBase.SaveChanges();
                return Json(new { success = true, message = "Owned Apartment deleted successfully" });
            }
        }

        /// <summary>
        /// Updates the list of owned apartments for a specific lodger
        /// </summary>
        /// <param name="id">The ID of the lodger</param>
        /// <param name="selectedApartments">New list of apartments owned for the lodger</param>
        /// <returns>JSON object indicating the success or failure of the update operation</returns>
        [HttpPost]
        public JsonResult UpdateApartments(string id, List<Apartment> selectedApartments)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase == null)
                {
                    return Json(null);
                }

                var lodger = dataBase.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(id));
                if (lodger == null)
                {
                    return Json("Ошибка при добавлении выбранных квартир");
                }

                var ownedApartmentsIds = lodger.OwnedApartments
                    .Where(x => selectedApartments
                    .Any(c => c.Id == x.ApartmentId))
                    .Select(x => x.ApartmentId).ToList();

                var lodgerApartments = dataBase.OwnedApartments
                    .Where(x => x.OwnerId == lodger.Id && ownedApartmentsIds.Contains(x.ApartmentId));
                dataBase.OwnedApartments.RemoveRange(lodgerApartments);
                dataBase.SaveChanges();

                foreach (var item in selectedApartments.Where(x => !ownedApartmentsIds.Contains(x.Id)))
                {
                    var apartment = dataBase.Apartments.Find(item.Id);
                    if (apartment != null)
                    {
                        dataBase.OwnedApartments.Add(new OwnedApartment { ApartmentId = apartment.Id, OwnerId = lodger.Id });
                        apartment.ResidentsNumber++;
                        dataBase.Apartments.Update(apartment);
                        dataBase.SaveChanges();
                    }
                }

                dataBase.SaveChanges();
                return Json("Выбранные квартиры успешно добавлены");
            }
        }
    }
}

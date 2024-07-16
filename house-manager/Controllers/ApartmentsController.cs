using house_manager.Models;
using house_manager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace house_manager.Controllers
{
    public class ApartmentsController : Controller
    {
        public JsonResult Get()
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var apartments = dataBase.Apartments.ToList();
                    return Json(apartments);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult UpdateApartments(string id, List<Apartment> selectedApartments)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var lodger = dataBase.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(id));
                    if (lodger != null)
                    {
                        var ownedApartmentsIds = lodger.OwnedApartments.Where(x => selectedApartments.Any(c => c.Id == x.ApartmentId)).Select(x => x.ApartmentId).ToList();

                        var lodgerApartments = dataBase.OwnedApartments.Where(x => x.OwnerId == lodger.Id);
                        foreach (var apartment in lodgerApartments)
                        {
                            if (!ownedApartmentsIds.Contains(apartment.ApartmentId))
                                dataBase.OwnedApartments.Remove(apartment);
                        }
                        dataBase.SaveChanges();

                        foreach (var item in selectedApartments)
                        {
                            if (ownedApartmentsIds.Contains(item.Id))
                                continue;
                            var apartment = dataBase.Apartments.Find(item.Id);
                            if (apartment != null)
                            {
                                dataBase.OwnedApartments.Add(new OwnedApartment { ApartmentId = apartment.Id, OwnerId = lodger.Id });
                                dataBase.SaveChanges();
                                apartment.ResidentsNumber++;
                                dataBase.Apartments.Update(apartment);
                                dataBase.SaveChanges();
                            }
                        }

                        dataBase.SaveChanges();
                        return Json("Выбранные квартиры успешно добавлены");
                    }
                    return Json("Ошибка при добавлении выбранных квартир");
                }
                return Json(null);
            }
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var ownedApartment = dataBase.OwnedApartments.Where(l => l.Id == int.Parse(id)).Include(x => x.Apartment).First();
                    return Json(ownedApartment);
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Update(string id, string ownershipPercentage, string ownerId)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    if (!float.TryParse(ownershipPercentage, out float temp))
                        return Json(new { success = false, message = "Вводите только числа" });
                    var ownedApartment = dataBase.OwnedApartments.Find(int.Parse(id));
                    var sum = dataBase.OwnedApartments.Where(x => x.ApartmentId == ownedApartment.ApartmentId && x.OwnerId != int.Parse(ownerId)).Sum(x => x.OwnershipPercentage);
                    if (sum + float.Parse(ownershipPercentage) > 100)
                        return Json(new { success = false, message = $"Ваша доля стоимости не должна превышать {(100 - sum)}%" });
                    ownedApartment.OwnershipPercentage = float.Parse(ownershipPercentage);
                    dataBase.OwnedApartments.Update(ownedApartment);
                    dataBase.SaveChanges();
                    return Json(new { success = true, message = "Owned Apartment updated successfully" });
                }
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Delete(string id, string ownerId)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dataBase = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                if (dataBase != null)
                {
                    var apartment = dataBase.Apartments.Find(int.Parse(id));
                    var lodger = dataBase.Lodgers.Include(l => l.OwnedApartments).FirstOrDefault(l => l.Id == int.Parse(ownerId));
                    if (lodger != null)
                    {
                        dataBase.OwnedApartments.Remove(dataBase.OwnedApartments.FirstOrDefault(x => x.ApartmentId == apartment.Id && x.OwnerId == lodger.Id));
                        dataBase.SaveChanges();
                        apartment.ResidentsNumber--;
                        dataBase.Apartments.Update(apartment);
                        dataBase.SaveChanges();
                    }
                    return Json(new { success = false, message = "Apartment not found" });
                }
                return Json(null);
            }
        }
    }
}

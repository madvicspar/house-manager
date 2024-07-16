using house_manager.Models;

namespace house_manager.Utilities
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationContext>();

                if (context.Apartments.Any())
                {
                    return;
                }

                context.Houses.Add(
                    new House
                    {
                        Address = "бул. Гагарина, 41",
                        ParkingPlacesNumber = 11
                    }
                );

                context.SaveChanges();

                var houseId = context.Houses.First().Id;

                for (int i = 0; i < 6; i++)
                {
                    context.Apartments.Add(
                        new Apartment
                        {
                            Number = (i + 1).ToString(),
                            HouseId = houseId,
                            ResidentsNumber = 0
                        });
                }

                for (int i = 0; i < 11; i++)
                {
                    context.ParkingSpaces.Add(
                        new ParkingSpace
                        {
                            Number = (i + 1).ToString(),
                            HouseId = houseId
                        });
                }

                context.Cars.Add(
                    new Car
                    {
                        Brand = "Toyota",
                        RegistrationNumber = "A123BC"
                    }
                );

                context.Cars.Add(
                    new Car
                    {
                        Brand = "BMW",
                        RegistrationNumber = "B456DE"
                    }
                );

                context.Lodgers.Add(
                    new Lodger
                    {
                        Surname = "Тестов",
                        Name = "Тест",
                        Pathronymic = "Тестович",
                        PassportNumber = "1234 567890"
                    }
                 );

                context.SaveChanges();
            }
        }
    }
}

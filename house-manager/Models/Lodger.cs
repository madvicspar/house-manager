using System.ComponentModel.DataAnnotations;

namespace house_manager.Models
{
    public class Lodger
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pathronymic { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}\s\d{6}$", ErrorMessage = "Пожалуйста, введите паспорт в формате XXXX XXXXXX")]
        public string PassportNumber { get; set; }
        public virtual List<Car> Cars { get; set; }
        public virtual List<Apartment> Apartments { get; set; }
        public virtual List<ParkingSpace> ParkingSpaces { get; set; }
    }
}

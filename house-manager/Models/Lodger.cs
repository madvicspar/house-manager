using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace house_manager.Models
{
    [Index(nameof(PassportNumber), IsUnique = true)]
    public class Lodger
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pathronymic { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}\s\d{6}$", ErrorMessage = "Пожалуйста, введите паспорт в формате XXXX XXXXXX")]
        public string PassportNumber { get; set; }
        [JsonIgnore]
        public virtual ICollection<OwnedCar>? OwnedCars { get; set; }
        public virtual ICollection<Apartment>? Apartments { get; set; }
        public virtual ICollection<Car>? Cars { get; set; }
        public virtual ICollection<OwnedParkingSpace>? OwnedParkingSpaces { get; set; }
        public virtual ICollection<OwnedApartment>? OwnedApartments { get; set; }
    }
}

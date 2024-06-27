using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace house_manager.Models
{
    [Index(nameof(RegistrationNumber), IsUnique = true)]
    public class Car
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{1}\d{3}[A-Z]{2}$", ErrorMessage = "Пожалуйста, введите номер в формате A123BC")]
        public string RegistrationNumber { get; set; }
        [Required]
        public string Brand { get; set; }
        public virtual ICollection<Lodger>? Owners { get; set; }
    }
}

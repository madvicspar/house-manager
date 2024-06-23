using System.ComponentModel.DataAnnotations;

namespace house_manager.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{1}\d{3}[A-Z]{2}$", ErrorMessage = "Пожалуйста, введите номер в формате A123BC")]
        public string RegistrationNumber { get; set; }
        [Required]
        public string Brand { get; set; }
        public virtual List<Lodger> Owners { get; set; }
    }
}

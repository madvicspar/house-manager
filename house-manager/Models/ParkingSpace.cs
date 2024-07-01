using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    [Index(nameof(Number), IsUnique = true)]
    public class ParkingSpace
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("HouseId")]
        public int HouseId { get; set; }
        public virtual House House { get; set; }
        [Required]
        public string Number { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    [Index(nameof(Number), IsUnique = true)]
    public class Apartment
    {
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }
        [ForeignKey("HouseId")]
        public int HouseId { get; set; }
        [Required]
        public virtual House House { get; set; }
        public int ResidentsNumber { get; set; }
        public virtual List<Lodger>? Owners { get; set; }
        public virtual List<OwnedApartment> OwnersOwned { get; set;}
    }
}

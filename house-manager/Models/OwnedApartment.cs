using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    public class OwnedApartment
    {
        public int Id { get; set; }
        [ForeignKey("ApartmentId")]
        public int ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }
        [ForeignKey("OwnerId")]
        public int OwnerId { get; set; }
        public virtual Lodger Owner { get; set; }
        public float OwnershipPercentage { get; set; }
    }
}

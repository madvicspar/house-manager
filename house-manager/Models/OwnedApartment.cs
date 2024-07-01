using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace house_manager.Models
{
    public class OwnedApartment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("ApartmentId")]
        public int ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }
        [ForeignKey("OwnerId")]
        public int OwnerId { get; set; }
        [JsonIgnore]
        public virtual Lodger Owner { get; set; }
        public float OwnershipPercentage { get; set; }
    }
}

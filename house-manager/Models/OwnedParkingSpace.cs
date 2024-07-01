using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    public class OwnedParkingSpace
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("ParkingSpaceId")]
        public int ParkingSpaceId { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }
        [ForeignKey("OwnerId")]
        public int OwnerId { get; set; }
        public virtual Lodger? Owner { get; set; }
    }
}

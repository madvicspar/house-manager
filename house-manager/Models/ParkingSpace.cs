using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        [ForeignKey("HouseId")]
        public int HouseId { get; set; }
        public virtual House House { get; set; }
        public string Number { get; set; }
        [ForeignKey("OwnerId")]
        public int OwnerId { get; set; }
        public virtual Lodger Owner { get; set; }
    }
}

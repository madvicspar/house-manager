using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    public class OwnedCar
    {
        public int Id { get; set; }
        [ForeignKey("CarId")]
        public int CarId { get; set; }
        public virtual Car Car { get; set; }
        [ForeignKey("OwnerId")]
        public int OwnerId { get; set; }
        public virtual Lodger Owner { get; set; }
    }
}

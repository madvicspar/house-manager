using System.ComponentModel.DataAnnotations.Schema;

namespace house_manager.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        public string Number { get; set; }
        [ForeignKey("HouseId")]
        public int HouseId { get; set; }
        public int ResidentsNumber { get; set; }
        public virtual House House { get; set; }
        public virtual List<Lodger> Owners { get; set; }
    }
}

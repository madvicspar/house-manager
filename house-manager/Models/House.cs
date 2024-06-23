﻿namespace house_manager.Models
{
    public class House
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int ParkingPlacesNumber { get; set; }
        public virtual List<Apartment> Apartments { get; set; }
        public virtual List<Lodger> Lodgers { get; set; }
        public virtual List<ParkingSpace> ParkingSpaces { get; set; }
    }
}
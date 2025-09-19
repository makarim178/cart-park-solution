using CarPark.Domain.Enums;
using System;

namespace CarPark.Domain.Entities
{
    public class ParkingRecord
    {
        public int Id { get; set; }
        public string VehicleReg { get; set; }
        public VehicleType VehicleType { get; set; }
        public int SpaceNumber { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public double VehicleCharge { get; set; }
    }
}

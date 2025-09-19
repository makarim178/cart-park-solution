using System;

namespace CarPark.Application.DTOs
{
    public class ParkResponse
    {
        public string VehicleReg { get; set; }
        public int SpaceNumber { get; set; }
        public DateTime TimeIn { get; set; }
    }
}

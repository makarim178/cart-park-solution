using System;

namespace CarPark.Application.DTOs
{
    public class ExitResponse
    {
        public string VehicleReg { get; set; }
        public double VehicleCharge { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
    }
}

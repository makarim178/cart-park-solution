namespace CarPark.Domain.Entities
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public int SpaceNumber { get; set; }
        public bool IsOccupied { get; set; }
    }
}

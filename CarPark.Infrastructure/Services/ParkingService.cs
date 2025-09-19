using CarPark.Application.DTOs;
using CarPark.Application.Interfaces;
using CarPark.Domain.Entities;
using CarPark.Domain.Enums;
using CarPark.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Application.Services
{
    public class ParkingService : IParkingService
    {
        private readonly CarParkDbContext _db;
        public ParkingService(CarParkDbContext db)
        {
            _db = db;
        }

        public async Task<ParkResponse> ParkVehicleAsync(ParkRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.VehicleReg))
                throw new ArgumentException("VehicleReg is required");

            if (!Enum.TryParse<VehicleType>(request.VehicleType, true, out var vt))
                throw new ArgumentException("VehicleType must be one of Small, Medium, Large");

            // prevent double parking
            var existing = await _db.ParkingRecords.FirstOrDefaultAsync(r => r.VehicleReg == request.VehicleReg && r.TimeOut == null);
            if (existing != null)
                throw new InvalidOperationException("Vehicle is already parked");

            var space = await _db.ParkingSpaces.OrderBy(s => s.SpaceNumber).FirstOrDefaultAsync(s => !s.IsOccupied);
            if (space == null)
                throw new InvalidOperationException("No available parking spaces");

            // allocate
            space.IsOccupied = true;
            var record = new ParkingRecord
            {
                VehicleReg = request.VehicleReg,
                VehicleType = vt,
                SpaceNumber = space.SpaceNumber,
                TimeIn = DateTime.UtcNow
            };

            _db.ParkingRecords.Add(record);
            await _db.SaveChangesAsync();

            return new ParkResponse
            {
                VehicleReg = record.VehicleReg,
                SpaceNumber = record.SpaceNumber,
                TimeIn = record.TimeIn
            };
        }

        public async Task<ParkingStatusResponse> GetStatusAsync()
        {
            var total = await _db.ParkingSpaces.CountAsync();
            var occupied = await _db.ParkingSpaces.CountAsync(s => s.IsOccupied);
            var available = total - occupied;
            return new ParkingStatusResponse { AvailableSpaces = available, OccupiedSpaces = occupied };
        }

        public async Task<ExitResponse> ExitVehicleAsync(ExitRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.VehicleReg))
                throw new ArgumentException("VehicleReg is required");

            var record = await _db.ParkingRecords
                .FirstOrDefaultAsync(r => r.VehicleReg == request.VehicleReg && r.TimeOut == null);

            if (record == null)
                throw new InvalidOperationException("No active parking record found for this vehicle");

            record.TimeOut = DateTime.UtcNow;
            // charge calc
            var timeSpan = record.TimeOut.Value - record.TimeIn;
            var minutes = Math.Ceiling(timeSpan.TotalMinutes);
            if (minutes < 0) minutes = 0;

            double ratePerMinute = record.VehicleType switch
            {
                VehicleType.Small => 0.10,
                VehicleType.Medium => 0.20,
                VehicleType.Large => 0.40,
                _ => 0.0
            };

            var baseCharge = ratePerMinute * minutes;
            // every 5 minutes an additional £1 (for each full 5-minute block)
            var extraBlocks = Math.Floor(minutes / 5.0);
            var extra = extraBlocks * 1.0;
            var total = baseCharge + extra;

            record.VehicleCharge = Math.Round(total, 2); // round to 2 decimals

            // free up the space
            var space = await _db.ParkingSpaces.FirstOrDefaultAsync(s => s.SpaceNumber == record.SpaceNumber);
            if (space != null) space.IsOccupied = false;

            await _db.SaveChangesAsync();

            return new ExitResponse
            {
                VehicleReg = record.VehicleReg,
                VehicleCharge = record.VehicleCharge,
                TimeIn = record.TimeIn,
                TimeOut = record.TimeOut.Value
            };
        }
    }
}

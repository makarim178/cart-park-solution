using System;
using System.Threading.Tasks;
using CarPark.Application.DTOs;
using CarPark.Application.Services;
using CarPark.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ParkingServiceTests
{
    private CarParkDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<CarParkDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var ctx = new CarParkDbContext(options);
        // seed 3 spaces
        for (int i = 1; i <= 3; i++)
            ctx.ParkingSpaces.Add(new CarPark.Domain.Entities.ParkingSpace { Id = i, SpaceNumber = i, IsOccupied = false });
        ctx.SaveChanges();
        return ctx;
    }

    [Fact]
    public async Task ParkVehicle_Allocates_FirstAvailable()
    {
        using var ctx = CreateContext("db1");
        var svc = new ParkingService(ctx);
        var r1 = await svc.ParkVehicleAsync(new ParkRequest { VehicleReg = "A1", VehicleType = "Small" });
        Assert.Equal(1, r1.SpaceNumber);

        var r2 = await svc.ParkVehicleAsync(new ParkRequest { VehicleReg = "B2", VehicleType = "Medium" });
        Assert.Equal(2, r2.SpaceNumber);
    }

    [Fact]
    public async Task ExitVehicle_Calculates_Charge_Correctly()
    {
        using var ctx = CreateContext("db2");
        // Manually add a record 10 minutes ago
        var space = await ctx.ParkingSpaces.FirstAsync();
        space.IsOccupied = true;
        var record = new CarPark.Domain.Entities.ParkingRecord
        {
            VehicleReg = "TEST10",
            VehicleType = CarPark.Domain.Enums.VehicleType.Medium,
            SpaceNumber = space.SpaceNumber,
            TimeIn = DateTime.UtcNow.AddMinutes(-10)
        };
        ctx.ParkingRecords.Add(record);
        await ctx.SaveChangesAsync();

        var svc = new ParkingService(ctx);
        var res = await svc.ExitVehicleAsync(new ExitRequest { VehicleReg = "TEST10" });

        // Medium: 0.20/min * 10 = 2.0; extra blocks = floor(10/5)=2 => +2 => total 4.0
        // Math.Floor is added to round down to nearest number, as time could vary slightly
        Assert.Equal(4.0, Math.Floor(res.VehicleCharge));
    }
}

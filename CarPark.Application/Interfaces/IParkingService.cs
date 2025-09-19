using System.Threading.Tasks;
using CarPark.Application.DTOs;

namespace CarPark.Application.Interfaces
{
    public interface IParkingService
    {
        Task<ParkResponse> ParkVehicleAsync(ParkRequest request);
        Task<ParkingStatusResponse> GetStatusAsync();
        Task<ExitResponse> ExitVehicleAsync(ExitRequest request);
    }
}

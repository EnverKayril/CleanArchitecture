using CleanArchitecture.Application.Features.CarFeatures.Commands.CreateCar;
using CleanArchitecture.Application.Features.CarFeatures.Queries.GetAllCar;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Shared.Pagination;

namespace CleanArchitecture.Application.Services;

public interface ICarService
{
    Task Createasync(CreateCarCommand request, CancellationToken cancellationToken);
    Task<PaginatedResult<Car>> GetAllAsync(GetAllCarQuery request, CancellationToken cancellationToken);
}

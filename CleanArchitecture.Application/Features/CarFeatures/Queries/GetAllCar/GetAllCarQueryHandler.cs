using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Shared.Pagination;
using MediatR;

namespace CleanArchitecture.Application.Features.CarFeatures.Queries.GetAllCar;

public sealed class GetAllCarQueryHandler : IRequestHandler<GetAllCarQuery, PaginatedResult<Car>>
{
    private readonly ICarService _carService;

    public GetAllCarQueryHandler(ICarService carService)
    {
        _carService = carService;
    }

    public async Task<PaginatedResult<Car>> Handle(GetAllCarQuery request, CancellationToken cancellationToken)
    {
        PaginatedResult<Car> cars = await _carService.GetAllAsync(request, cancellationToken);
        return cars;
    }
}

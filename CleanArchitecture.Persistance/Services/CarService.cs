using AutoMapper;
using CleanArchitecture.Application.Common.Pagination;
using CleanArchitecture.Application.Features.CarFeatures.Commands.CreateCar;
using CleanArchitecture.Application.Features.CarFeatures.Queries.GetAllCar;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Domain.Shared.Pagination;
using CleanArchitecture.Persistance.Context;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistance.Services;

public sealed class CarService : ICarService
{
    private readonly AppDbContext _context;
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CarService(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, ICarRepository carRepository)
    {
        _context = context;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _carRepository = carRepository;
    }

    public async Task Createasync(CreateCarCommand request, CancellationToken cancellationToken)
    {
        Car car = _mapper.Map<Car>(request);

        //await _context.Set<Car>().AddAsync(car, cancellationToken);
        //await _context.SaveChangesAsync(cancellationToken);

        await _carRepository.AddAsync(car, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Car>> GetAllAsync(GetAllCarQuery request, CancellationToken cancellationToken)
    {
        var query = _carRepository.GetAll().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{request.Search}%"));

        var sortableWhitelist = new[] { "Name", "EnginePower", "Id" };

        return await query.ToPaginatedResultAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            ct: cancellationToken,
            sortableWhitelist: sortableWhitelist,
            defaultSortBy: "Id",                    // entity'nde yoksa uygun bir alan ver: "CreatedDate" gibi
            defaultDesc: false,
            sortBy: null,
            desc: true,
            includeTotalCount: true
        );
    }
}

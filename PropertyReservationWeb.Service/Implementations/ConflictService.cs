using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Extensions;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels;
using PropertyReservationWeb.Domain.ViewModels.Conflict;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class ConflictService : IConflictService
    {
        private readonly IBaseRepository<Conflict> _conflictRepository;
        private const int pageSize = 21;

        public ConflictService(IBaseRepository<Conflict> conflictRepository) 
        {
            _conflictRepository = conflictRepository;
        }

        public async Task<IBaseResponse<PaginatedViewModelResponse<ConflictViewModel, ConflictFilterModel>>> GetConflicts(int page, ConflictFilterModel filterModel)
        {
            try
            {
                var query = _conflictRepository.GetAll();

                query.OrderByDescending(c => c.DateCreated);

                if (filterModel.RentalRequestId.HasValue)
                    query = query.Where(c => c.RentalRequestId == filterModel.RentalRequestId.Value);

                if (filterModel.ResolvedByAdminId.HasValue)
                    query = query.Where(c => c.ResolvedByAdminId == filterModel.ResolvedByAdminId.Value);

                if (filterModel.Status.HasValue)
                    query = query.Where(c => c.Status == filterModel.Status.Value);

                var totalItems = await query.CountAsync();
                var conflicts = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new ConflictViewModel(
                        c.Id,
                        c.RentalRequestId,
                        c.CreatedByUserId,
                        c.ResolvedByAdminId,
                        c.Description,
                        c.Status.GetDisplayName(),
                        c.DateCreated,
                        c.DateResolved
                    ))
                    .ToListAsync();

                return new BaseResponse<PaginatedViewModelResponse<ConflictViewModel, ConflictFilterModel>>
                {
                    Data = new PaginatedViewModelResponse<ConflictViewModel, ConflictFilterModel>(
                        conflicts,
                       (int)totalItems/pageSize,
                        filterModel),
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedViewModelResponse<ConflictViewModel, ConflictFilterModel>>
                {
                    Description = $"[GetConflicts]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<ConflictViewModel>> RejectedConflict(long id, long idUser)
        {
            try
            {
                var conflict = await _conflictRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (conflict == null)
                {
                    return new BaseResponse<ConflictViewModel>
                    {
                        Description = "Конфликт не найден.",
                        StatusCode = StatusCode.ConflictNotFound
                    };
                }

                conflict.Status = ConflictStatus.Rejected;
                conflict.ResolvedByAdminId = idUser;
                conflict.DateResolved = DateTime.UtcNow;

                await _conflictRepository.Update(conflict);

                return new BaseResponse<ConflictViewModel>
                {
                    Data = new ConflictViewModel(
                        conflict.Id,
                        conflict.RentalRequestId,
                        conflict.CreatedByUserId,
                        conflict.ResolvedByAdminId,
                        conflict.Description,
                        conflict.Status.ToString(),
                        conflict.DateCreated,
                        conflict.DateResolved
                    ),
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ConflictViewModel>
                {
                    Description = $"[RejectedConflict]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<ConflictViewModel>> ResolvedConflict(long id, long idUser)
        {
            try
            {
                var conflict = await _conflictRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c=>c.Id == id);
                if (conflict == null)
                {
                    return new BaseResponse<ConflictViewModel>
                    {
                        Description = "Конфликт не найден.",
                        StatusCode = StatusCode.ConflictNotFound
                    };
                }

                conflict.Status = ConflictStatus.Resolved;
                conflict.ResolvedByAdminId = idUser;
                conflict.DateResolved = DateTime.UtcNow;

                await _conflictRepository.Update(conflict);

                return new BaseResponse<ConflictViewModel>
                {
                    Data = new ConflictViewModel(
                        conflict.Id,
                        conflict.RentalRequestId,
                        conflict.CreatedByUserId,
                        conflict.ResolvedByAdminId,
                        conflict.Description,
                        conflict.Status.ToString(),
                        conflict.DateCreated,
                        conflict.DateResolved
                    ),
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ConflictViewModel>
                {
                    Description = $"[ResolvedConflict]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}

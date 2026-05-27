using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterService(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<ApiResponse<PagedResponse<object>>> GetSemestersAsync(QueryRequest request)
        {

            var query = _semesterRepository.GetQueryable();

            if (!string.IsNullOrEmpty(request.Expand))
            {
                var expands = request.Expand.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var expand in expands)
                {
                    var trimmedExpand = expand.Trim();
                    if (!string.IsNullOrEmpty(trimmedExpand))
                    {
                        var parts = trimmedExpand.Split('.');
                        var pascalCaseParts = parts.Select(p => char.ToUpper(p[0]) + p.Substring(1));
                        var pascalCaseExpand = string.Join(".", pascalCaseParts);
                        query = query.Include(pascalCaseExpand);
                    }
                }
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(s => s.SemesterName.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = query.OrderBy(request.SortBy);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            List<object> resultList;
            if (!string.IsNullOrEmpty(request.Select))
            {
                var selectQuery = items.AsQueryable().Select($"new({request.Select})");
                resultList = selectQuery.ToDynamicList<object>();
            }
            else
            {
                resultList = items.Select(s => new SemesterDto
                {
                    SemesterId = s.SemesterId,
                    SemesterName = s.SemesterName,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                }).Cast<object>().ToList();
            }

            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);
            var pagedResponse = new PagedResponse<object>(resultList, request.Page, request.PageSize, totalItems, totalPages);

            return ApiResponse<PagedResponse<object>>.Ok(pagedResponse);

        }

        public async Task<ApiResponse<SemesterDto>> GetSemesterByIdAsync(int id)
        {

            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
                throw new System.Collections.Generic.KeyNotFoundException("Semester not found.");

            var dto = new SemesterDto
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };

            return ApiResponse<SemesterDto>.Ok(dto);

        }

        public async Task<ApiResponse<SemesterDto>> CreateSemesterAsync(SemesterDto request)
        {

            var semester = new Semester
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await _semesterRepository.InsertAsync(semester);
            await _semesterRepository.SaveChangesAsync();

            request.SemesterId = semester.SemesterId;
            return ApiResponse<SemesterDto>.Ok(request, "Semester created successfully.");

        }

        public async Task<ApiResponse<SemesterDto>> UpdateSemesterAsync(int id, SemesterDto request)
        {

            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
                throw new System.Collections.Generic.KeyNotFoundException("Semester not found.");

            semester.SemesterName = request.SemesterName;
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;

            _semesterRepository.Update(semester);
            await _semesterRepository.SaveChangesAsync();

            request.SemesterId = semester.SemesterId;
            return ApiResponse<SemesterDto>.Ok(request, "Semester updated successfully.");

        }

        public async Task<ApiResponse<bool>> DeleteSemesterAsync(int id)
        {

            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
                throw new System.Collections.Generic.KeyNotFoundException("Semester not found.");

            _semesterRepository.Delete(semester);
            await _semesterRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Semester deleted successfully.");

        }
    }
}

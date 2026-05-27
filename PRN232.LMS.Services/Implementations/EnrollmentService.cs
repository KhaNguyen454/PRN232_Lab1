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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ApiResponse<PagedResponse<object>>> GetEnrollmentsAsync(QueryRequest request)
        {

            var query = _enrollmentRepository.GetQueryable();

            // Expand (Include related entities)
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

            // Search
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(e => e.Status.ToLower().Contains(keyword));
            }

            // Sort
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = query.OrderBy(request.SortBy);
            }

            var totalItems = await query.CountAsync();

            // Paging
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Map to DTOs or Select specified fields
            List<object> resultList;

            if (!string.IsNullOrEmpty(request.Select))
            {
                var selectQuery = items.AsQueryable().Select($"new({request.Select})");
                resultList = selectQuery.ToDynamicList<object>();
            }
            else
            {
                // Map to DTO
                resultList = items.Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Course = e.Course != null ? new CourseDto
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId
                    } : null,
                    Student = e.Student != null ? new StudentDto
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth
                    } : null
                }).Cast<object>().ToList();
            }

            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);
            var pagedResponse = new PagedResponse<object>(resultList, request.Page, request.PageSize, totalItems, totalPages);

            return ApiResponse<PagedResponse<object>>.Ok(pagedResponse);

        }

        public async Task<ApiResponse<EnrollmentDto>> GetEnrollmentByIdAsync(int id)
        {

            var enrollment = await _enrollmentRepository.GetQueryable()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Enrollment not found.");
            }

            var dto = new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Course = enrollment.Course != null ? new CourseDto
                {
                    CourseId = enrollment.Course.CourseId,
                    CourseName = enrollment.Course.CourseName,
                    SemesterId = enrollment.Course.SemesterId
                } : null,
                Student = enrollment.Student != null ? new StudentDto
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    DateOfBirth = enrollment.Student.DateOfBirth
                } : null
            };

            return ApiResponse<EnrollmentDto>.Ok(dto);

        }
        public async Task<ApiResponse<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto request)
        {

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            await _enrollmentRepository.InsertAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            request.EnrollmentId = enrollment.EnrollmentId;
            return ApiResponse<EnrollmentDto>.Ok(request, "Enrollment created successfully.");

        }

        public async Task<ApiResponse<EnrollmentDto>> UpdateEnrollmentAsync(int id, EnrollmentDto request)
        {

            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                throw new System.Collections.Generic.KeyNotFoundException("Enrollment not found.");

            enrollment.StudentId = request.StudentId;
            enrollment.CourseId = request.CourseId;
            enrollment.EnrollDate = request.EnrollDate;
            enrollment.Status = request.Status;

            _enrollmentRepository.Update(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            request.EnrollmentId = enrollment.EnrollmentId;
            return ApiResponse<EnrollmentDto>.Ok(request, "Enrollment updated successfully.");

        }

        public async Task<ApiResponse<bool>> DeleteEnrollmentAsync(int id)
        {

            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                throw new System.Collections.Generic.KeyNotFoundException("Enrollment not found.");

            _enrollmentRepository.Delete(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Enrollment deleted successfully.");

        }
    }
}

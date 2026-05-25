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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<ApiResponse<PagedResponse<object>>> GetCoursesAsync(QueryRequest request)
        {
            try
            {
                var query = _courseRepository.GetQueryable();

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
                    query = query.Where(c => c.CourseName.ToLower().Contains(keyword));
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
                    resultList = items.Select(c => new CourseDto
                    {
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        SemesterId = c.SemesterId,
                        Semester = c.Semester != null ? new SemesterDto
                        {
                            SemesterId = c.Semester.SemesterId,
                            SemesterName = c.Semester.SemesterName,
                            StartDate = c.Semester.StartDate,
                            EndDate = c.Semester.EndDate
                        } : null
                    }).Cast<object>().ToList();
                }

                var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);
                var pagedResponse = new PagedResponse<object>(resultList, request.Page, request.PageSize, totalItems, totalPages);

                return ApiResponse<PagedResponse<object>>.Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResponse<object>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = await _courseRepository.GetQueryable()
                    .Include(c => c.Semester)
                    .FirstOrDefaultAsync(c => c.CourseId == id);

                if (course == null)
                    return ApiResponse<CourseDto>.Fail("Course not found."); 

                var dto = new CourseDto
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    SemesterId = course.SemesterId,
                    Semester = course.Semester != null ? new SemesterDto
                    {
                        SemesterId = course.Semester.SemesterId,
                        SemesterName = course.Semester.SemesterName,
                        StartDate = course.Semester.StartDate,
                        EndDate = course.Semester.EndDate
                    } : null
                };

                return ApiResponse<CourseDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> CreateCourseAsync(CourseDto request)
        {
            try
            {
                var course = new Course
                {
                    CourseName = request.CourseName,
                    SemesterId = request.SemesterId
                };

                await _courseRepository.InsertAsync(course);
                await _courseRepository.SaveChangesAsync();

                request.CourseId = course.CourseId;
                return ApiResponse<CourseDto>.Ok(request, "Course created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> UpdateCourseAsync(int id, CourseDto request)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                    return ApiResponse<CourseDto>.Fail("Course not found.");

                course.CourseName = request.CourseName;
                course.SemesterId = request.SemesterId;

                _courseRepository.Update(course);
                await _courseRepository.SaveChangesAsync();

                request.CourseId = course.CourseId;
                return ApiResponse<CourseDto>.Ok(request, "Course updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                    return ApiResponse<bool>.Fail("Course not found.");

                _courseRepository.Delete(course);
                await _courseRepository.SaveChangesAsync();

                return ApiResponse<bool>.Ok(true, "Course deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}

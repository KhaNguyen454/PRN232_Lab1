using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Make sure to add System.Linq.Dynamic.Core package
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<ApiResponse<PagedResponse<object>>> GetStudentsAsync(QueryRequest request)
        {
            try
            {
                var query = _studentRepository.GetQueryable();

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
                    query = query.Where(s => s.FullName.ToLower().Contains(keyword) || s.Email.ToLower().Contains(keyword));
                }

                // Sort
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = query.OrderBy(request.SortBy); // Requires System.Linq.Dynamic.Core
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
                    // Basic implementation of Selection (using Dynamic Linq or manual mapping)
                    // For a robust implementation, AutoMapper ProjectTo is recommended.
                    // Using Dynamic Linq for Selection
                    var selectQuery = items.AsQueryable().Select($"new({request.Select})");
                    resultList = selectQuery.ToDynamicList<object>();
                }
                else
                {
                    // Map to DTO
                    resultList = items.Select(s => new StudentDto
                    {
                        StudentId = s.StudentId,
                        FullName = s.FullName,
                        Email = s.Email,
                        DateOfBirth = s.DateOfBirth,
                        Enrollments = s.Enrollments?.Select(e => new EnrollmentDto
                        {
                            EnrollmentId = e.EnrollmentId,
                            CourseId = e.CourseId,
                            EnrollDate = e.EnrollDate,
                            Status = e.Status
                        }).ToList()
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

        public async Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await _studentRepository.GetQueryable()
                    .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                if (student == null)
                {
                    return ApiResponse<StudentDto>.Fail("Student not found."); // We'll map this to 404 in controller
                }

                var dto = new StudentDto
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    Enrollments = student.Enrollments.Select(e => new EnrollmentDto
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        EnrollDate = e.EnrollDate,
                        Status = e.Status,
                        Course = e.Course != null ? new CourseDto
                        {
                            CourseId = e.Course.CourseId,
                            CourseName = e.Course.CourseName,
                            SemesterId = e.Course.SemesterId
                        } : null
                    }).ToList()
                };

                return ApiResponse<StudentDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.Fail($"An error occurred: {ex.Message}");
            }
        }
        public async Task<ApiResponse<StudentDto>> CreateStudentAsync(StudentDto request)
        {
            try
            {
                var student = new Student
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth
                };

                await _studentRepository.InsertAsync(student);
                await _studentRepository.SaveChangesAsync();

                request.StudentId = student.StudentId;
                return ApiResponse<StudentDto>.Ok(request, "Student created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> UpdateStudentAsync(int id, StudentDto request)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                    return ApiResponse<StudentDto>.Fail("Student not found.");

                student.FullName = request.FullName;
                student.Email = request.Email;
                student.DateOfBirth = request.DateOfBirth;

                _studentRepository.Update(student);
                await _studentRepository.SaveChangesAsync();

                request.StudentId = student.StudentId;
                return ApiResponse<StudentDto>.Ok(request, "Student updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                    return ApiResponse<bool>.Fail("Student not found.");

                _studentRepository.Delete(student);
                await _studentRepository.SaveChangesAsync();

                return ApiResponse<bool>.Ok(true, "Student deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}

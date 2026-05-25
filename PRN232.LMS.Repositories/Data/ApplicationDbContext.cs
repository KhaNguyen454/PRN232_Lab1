using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Models;
using System;
using System.Collections.Generic;

namespace PRN232.LMS.Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId);
                entity.Property(e => e.SemesterName).HasColumnType("nvarchar(100)").IsRequired();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseName).HasColumnType("nvarchar(100)").IsRequired();
                entity.HasOne(e => e.Semester)
                    .WithMany(s => s.Courses)
                    .HasForeignKey(e => e.SemesterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.Property(e => e.SubjectCode).HasColumnType("varchar(20)").IsRequired();
                entity.Property(e => e.SubjectName).HasColumnType("nvarchar(100)").IsRequired();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.FullName).HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(e => e.Email).HasColumnType("varchar(100)").IsRequired();
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasColumnType("varchar(20)").IsRequired();
                
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed 5 Semesters
            var semesters = new List<Semester>();
            for (int i = 1; i <= 5; i++)
            {
                semesters.Add(new Semester
                {
                    SemesterId = i,
                    SemesterName = $"Semester {i}",
                    StartDate = new DateTime(2023, 1, 1).AddMonths((i - 1) * 4),
                    EndDate = new DateTime(2023, 4, 30).AddMonths((i - 1) * 4)
                });
            }
            modelBuilder.Entity<Semester>().HasData(semesters);

            // Seed 10 Subjects
            var subjects = new List<Subject>();
            for (int i = 1; i <= 10; i++)
            {
                subjects.Add(new Subject
                {
                    SubjectId = i,
                    SubjectCode = $"SUBJ{i:D3}",
                    SubjectName = $"Subject {i}",
                    Credit = 3
                });
            }
            modelBuilder.Entity<Subject>().HasData(subjects);

            // Seed 20 Courses
            var courses = new List<Course>();
            for (int i = 1; i <= 20; i++)
            {
                courses.Add(new Course
                {
                    CourseId = i,
                    CourseName = $"Course {i}",
                    SemesterId = (i % 5) + 1 // Assign across 5 semesters
                });
            }
            modelBuilder.Entity<Course>().HasData(courses);

            // Seed 50 Students
            var students = new List<Student>();
            for (int i = 1; i <= 50; i++)
            {
                students.Add(new Student
                {
                    StudentId = i,
                    FullName = $"Student {i}",
                    Email = $"student{i}@lms.edu",
                    DateOfBirth = new DateTime(2000, 1, 1).AddDays(i)
                });
            }
            modelBuilder.Entity<Student>().HasData(students);

            // Seed 500 Enrollments
            var enrollments = new List<Enrollment>();
            var random = new Random(123); // Seed for deterministic data
            for (int i = 1; i <= 500; i++)
            {
                enrollments.Add(new Enrollment
                {
                    EnrollmentId = i,
                    StudentId = random.Next(1, 51),
                    CourseId = random.Next(1, 21),
                    EnrollDate = new DateTime(2023, 1, 1).AddDays(random.Next(1, 365)),
                    Status = i % 2 == 0 ? "Active" : "Completed"
                });
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}

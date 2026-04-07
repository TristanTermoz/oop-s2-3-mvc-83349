using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace VgcCollege.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = new[] { "Administrator", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create an admin user
            var adminEmail = "admin@vgc.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Administrator");
            }

            // Create a faculty user
            var facultyEmail = "faculty@vgc.local";
            var faculty = await userManager.FindByEmailAsync(facultyEmail);
            if (faculty == null)
            {
                faculty = new IdentityUser { UserName = facultyEmail, Email = facultyEmail, EmailConfirmed = true };
                await userManager.CreateAsync(faculty, "Faculty123!");
                await userManager.AddToRoleAsync(faculty, "Faculty");
            }

            // Create a student user
            var studentEmail = "student@vgc.local";
            var student = await userManager.FindByEmailAsync(studentEmail);
            if (student == null)
            {
                student = new IdentityUser { UserName = studentEmail, Email = studentEmail, EmailConfirmed = true };
                await userManager.CreateAsync(student, "Student123!");
                await userManager.AddToRoleAsync(student, "Student");
            }

            // Create a second student user
            var student2Email = "student2@vgc.local";
            var student2 = await userManager.FindByEmailAsync(student2Email);
            if (student2 == null)
            {
                student2 = new IdentityUser { UserName = student2Email, Email = student2Email, EmailConfirmed = true };
                await userManager.CreateAsync(student2, "Student123!");
                await userManager.AddToRoleAsync(student2, "Student");
            }

            // Seed domain data if not already present
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (!await db.Branches.AnyAsync())
            {
                var branch = new Models.Branch { Name = "Dublin Campus", Address = "1 College St" };
                db.Branches.Add(branch);

                var facultyProfile = new Models.FacultyProfile { IdentityUserId = faculty.Id, Name = "Dr Faculty", Email = facultyEmail, Phone = "+353123" };
                db.FacultyProfiles.Add(facultyProfile);

                var studentProfile = new Models.StudentProfile { IdentityUserId = student.Id, Name = "Student One", Email = studentEmail, Phone = "+353456", Address = "Student Address", DateOfBirth = new DateTime(2000,1,1), StudentNumber = "S1001" };
                db.StudentProfiles.Add(studentProfile);

                var studentProfile2 = new Models.StudentProfile { IdentityUserId = student2.Id, Name = "Student Two", Email = student2Email, Phone = "+353789", Address = "Student2 Address", DateOfBirth = new DateTime(2001,2,2), StudentNumber = "S1002" };
                db.StudentProfiles.Add(studentProfile2);

                var course = new Models.Course { Name = "Computer Science 101", Branch = branch, StartDate = DateTime.Today.AddMonths(-1), EndDate = DateTime.Today.AddMonths(5), Faculty = facultyProfile };
                db.Courses.Add(course);

                await db.SaveChangesAsync();

                var enrol = new Models.CourseEnrolment { Student = studentProfile, Course = course, EnrolDate = DateTime.Today.AddDays(-10), Status = "Active" };
                db.CourseEnrolments.Add(enrol);

                var enrol2 = new Models.CourseEnrolment { Student = studentProfile2, Course = course, EnrolDate = DateTime.Today.AddDays(-8), Status = "Active" };
                db.CourseEnrolments.Add(enrol2);

                var attendance = new Models.AttendanceRecord { CourseEnrolment = enrol, SessionDate = DateTime.Today.AddDays(-7), Present = true };
                db.AttendanceRecords.Add(attendance);

                var assignment = new Models.Assignment { Course = course, Title = "Assignment 1", MaxScore = 100, DueDate = DateTime.Today.AddDays(7) };
                db.Assignments.Add(assignment);

                var assignmentResult = new Models.AssignmentResult { Assignment = assignment, Student = studentProfile, Score = 85, Feedback = "Good" };
                db.AssignmentResults.Add(assignmentResult);

                var assignmentResult2 = new Models.AssignmentResult { Assignment = assignment, Student = studentProfile2, Score = 78, Feedback = "Satisfactory" };
                db.AssignmentResults.Add(assignmentResult2);

                var exam = new Models.Exam { Course = course, Title = "Midterm", Date = DateTime.Today.AddDays(14), MaxScore = 100, ResultsReleased = false };
                db.Exams.Add(exam);

                var examResult = new Models.ExamResult { Exam = exam, Student = studentProfile, Score = 72, Grade = "C" };
                db.ExamResults.Add(examResult);

                var examResult2 = new Models.ExamResult { Exam = exam, Student = studentProfile2, Score = 65, Grade = "D" };
                db.ExamResults.Add(examResult2);

                await db.SaveChangesAsync();
            }
        }
    }
}

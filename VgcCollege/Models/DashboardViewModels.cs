using System.Collections.Generic;

namespace VgcCollege.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<CourseEnrolment> Enrolments { get; set; }
        public IEnumerable<FacultyProfile> FacultyProfiles { get; set; }
        public IEnumerable<StudentProfile> StudentProfiles { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
        public IEnumerable<Exam> Exams { get; set; }
    }

    public class FacultyDashboardViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<CourseEnrolment> Enrolments { get; set; }
        public IEnumerable<AssignmentResult> AssignmentResults { get; set; }
    }

    public class StudentDashboardViewModel
    {
        public StudentProfile Student { get; set; }
        public IEnumerable<CourseEnrolment> Enrolments { get; set; }
        public IEnumerable<AssignmentResult> AssignmentResults { get; set; }
        public IEnumerable<ExamResult> ExamResults { get; set; }
    }
}

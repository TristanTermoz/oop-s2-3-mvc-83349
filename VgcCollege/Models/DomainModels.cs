using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int? FacultyProfileId { get; set; }
        public FacultyProfile Faculty { get; set; }

        public ICollection<CourseEnrolment> Enrolments { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }

    public class StudentProfile
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(50)]
        public string StudentNumber { get; set; }

        public ICollection<CourseEnrolment> Enrolments { get; set; }
        public ICollection<AssignmentResult> AssignmentResults { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
    }

    public class FacultyProfile
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public ICollection<Course> Courses { get; set; }
    }

    public class CourseEnrolment
    {
        public int Id { get; set; }

        public int StudentProfileId { get; set; }
        public StudentProfile Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrolDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
    }

    public class AttendanceRecord
    {
        public int Id { get; set; }

        public int CourseEnrolmentId { get; set; }
        public CourseEnrolment CourseEnrolment { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        public bool Present { get; set; }
    }

    public class Assignment
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        public decimal MaxScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public ICollection<AssignmentResult> Results { get; set; }
    }

    public class AssignmentResult
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public int StudentProfileId { get; set; }
        public StudentProfile Student { get; set; }

        public decimal Score { get; set; }

        public string Feedback { get; set; }
    }

    public class Exam
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public decimal MaxScore { get; set; }

        public bool ResultsReleased { get; set; }

        public ICollection<ExamResult> Results { get; set; }
    }

    public class ExamResult
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int StudentProfileId { get; set; }
        public StudentProfile Student { get; set; }

        public decimal Score { get; set; }

        [StringLength(10)]
        public string Grade { get; set; }
    }
}

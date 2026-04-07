using System;
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Models
{
    public class StudentCreateViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string StudentNumber { get; set; }

        public int[] SelectedCourseIds { get; set; }
    }

    public class StudentEditViewModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string StudentNumber { get; set; }

        public int[] SelectedCourseIds { get; set; }
    }
}

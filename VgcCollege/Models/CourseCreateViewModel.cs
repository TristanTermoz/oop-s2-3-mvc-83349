using System;
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Models
{
    public class CourseCreateViewModel
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int? BranchId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}

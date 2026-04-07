using System;
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Models
{
    public class AssignmentCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int? CourseId { get; set; }

        [Display(Name = "Max Score")]
        public decimal MaxScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
    }

    public class ExamCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int? CourseId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Max Score")]
        public decimal MaxScore { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VgcCollege.Models;
using Xunit;

namespace TestProject1.Tests
{
    public class ModelValidationTests
    {
        private IList<ValidationResult> Validate(object model)
        {
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void StudentCreateViewModel_Invalid_WhenMissingRequiredFields()
        {
            var m = new StudentCreateViewModel();
            var results = Validate(m);

            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Assignment_Requires_Title()
        {
            var a = new Assignment { Title = null };
            var results = Validate(a);

            Assert.Contains(results, r => r.MemberNames.Contains("Title"));
        }

        [Fact]
        public void StudentEditViewModel_Valid_WithRequiredFields()
        {
            var m = new StudentEditViewModel { Email = "s@x.com", Name = "Student" };
            var results = Validate(m);
            Assert.Empty(results);
        }

        [Fact]
        public void Branch_Requires_Name()
        {
            var b = new Branch { Name = null };
            var results = Validate(b);
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Course_Requires_Name()
        {
            var c = new Course { Name = null };
            var results = Validate(c);
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void AssignmentCreateViewModel_Requires_Title_And_Course()
        {
            var m = new AssignmentCreateViewModel();
            var results = Validate(m);
            // Title should be reported by validation
            Assert.Contains(results, r => r.ErrorMessage?.Contains("Title") == true);

            // CourseId should be marked required via attribute
            var hasRequired = typeof(AssignmentCreateViewModel).GetProperty("CourseId").GetCustomAttributes(typeof(RequiredAttribute), false).Any();
            Assert.True(hasRequired);
        }

        [Fact]
        public void ExamCreateViewModel_Requires_Title_And_Course()
        {
            var m = new ExamCreateViewModel();
            var results = Validate(m);
            Assert.Contains(results, r => r.ErrorMessage?.Contains("Title") == true);
            var hasRequired = typeof(ExamCreateViewModel).GetProperty("CourseId").GetCustomAttributes(typeof(RequiredAttribute), false).Any();
            Assert.True(hasRequired);
        }

        [Fact]
        public void FacultyProfile_Requires_Identity_Name_Email()
        {
            var f = new FacultyProfile { IdentityUserId = null, Name = null, Email = null };
            var results = Validate(f);
            Assert.Contains(results, r => r.MemberNames.Contains("IdentityUserId"));
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void StudentProfile_Requires_Identity_Name_Email()
        {
            var s = new StudentProfile { IdentityUserId = null, Name = null, Email = null };
            var results = Validate(s);
            Assert.Contains(results, r => r.MemberNames.Contains("IdentityUserId"));
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS.Models.ViewModel
{
    public class ProfileVM
    {
        [Key]
        [Required]
        public int UserId { get; set; }

        [Display(Name = "First Name :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Display(Name = "Email Address :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters is required")]
        public string Password { get; set; }

        [Display(Name = "New Password :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters is required")]
        public string NewPassword { get; set; }

        [NotMapped]
        [Display(Name = "Confirm Password :")]
        [Required(ErrorMessage = "Confirm Password required")]
        [CompareAttribute("NewPassword", ErrorMessage = "Password doesn't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassowrd { get; set; }

        [Required]
        public int DeptId { get; set; }

        [Display(Name = "Department Name :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Name is required")]
        public string DeptName { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Display(Name = "Document Name :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Document name is required")]
        public string DocumentName { get; set; }

        [Display(Name = "Document Path :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Document path is required")]
        public string DocumentPath { get; set; }

        [Display(Name = "Document Details :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Document details is required")]
        public string DocumentDetails { get; set; }
    }
}
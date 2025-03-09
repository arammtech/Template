using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace Template.Service.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [DisplayName("الاسم الأول ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [DisplayName("الاسم الأخير")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [DisplayName("البريد الإلكتروني")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; }

        public string? UserName { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "رقم الهاتف يجب أن يتألف من 11 خانة حصرا")]
        public string? Phone { get; set; }

        // make here validation
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DisplayName("كلمة المرور")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "يجب أن تحتوي كلمة المرور على 8 أحرف على الأقل، حرف كبير، رقم، وحرف خاص.")]
        public string? Password { get; set; }

        public string? ImagePath { get; set; } 

        [Required(ErrorMessage = "الدور مطلوب")]
        [DisplayName("الدور")]
        public List<string> Role { get; set; }
        public bool? IsLocked { get; set; }
    }
}

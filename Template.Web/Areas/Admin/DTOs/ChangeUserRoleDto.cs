using System.ComponentModel.DataAnnotations;
using Template.Domain.Identity;

namespace Template.Web.Areas.Admin.ViewModels
{
    public class ChangeUserRoleDto
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public string oldRole { get; set; }

        public string? newRole { get; set; }

        public List<ApplicationRole> Roles { get; set; } = [];
    }
}

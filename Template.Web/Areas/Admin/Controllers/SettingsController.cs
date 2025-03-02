using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Utilities.Identity;

namespace Template.Web.Areas.Admin.Controllers
{
    public class SettingsController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = AppUserRoles.RoleAdmin)]
        public IActionResult Index()
        {
            return View();
        }
    }
}

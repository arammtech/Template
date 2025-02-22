using Microsoft.AspNetCore.Mvc;
using Template.Service.DTOs;
using Template.Service.IService;

namespace Template.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var departments = _departmentService.GetAll();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DepartmentDto departmentDto)
        {
            if (ModelState.IsValid)
            {
                 _departmentService.Add(departmentDto);
                 _departmentService.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentDto);
        }
    }
}

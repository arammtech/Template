using Microsoft.AspNetCore.Mvc;
using Template.Service.DTOs;
using Template.Service.Interfaces;

namespace Template.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
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
                var result =  _departmentService.Add(departmentDto);
                if(result.IsSuccess)
                {
                    TempData["success"] = "تم إضافة المنتج للسلة بنجاح!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["success"] = "لم يتم إضافة المنتج للسلة بنجاح !";
                    ModelState.AddModelError("", result.ErrorMessage);
                }
            }
            return View(departmentDto);
        }
    }
}

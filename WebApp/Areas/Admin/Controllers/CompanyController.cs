using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Collections.Generic;
using Web.DataAccess;
using Web.DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Web.Utility;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompany = _unitOfWork.Company.GetAll().ToList();
            return View(objCompany);
        }

        public IActionResult CreateCompany()
        {
            return View("UpsertCompany", new Company());
        }

        [HttpPost]
        public IActionResult CreateCompany(Company obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }

            return View("UpsertCompany", obj);
        }

        public IActionResult EditCompany(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Edit Error Occurred. Company Not Found!";
                return NotFound();
            }

            var catagoryFromDb = _unitOfWork.Company.Get(c => c.Id == id);

            if (catagoryFromDb == null)
            {
                return NotFound();
            }

            return View("UpsertCompany", catagoryFromDb);
        }

        [HttpPost]
        public IActionResult EditCompany(Company obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Company Updated Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Edit Error Occurred. Failed!";
            return View("UpsertCompany" ,obj);
        }

        [HttpPost]
        public IActionResult DeleteCompany(int id)
        {
            var catagoryFromDb = _unitOfWork.Company.Get(c => c.Id == id);

            if (catagoryFromDb == null)
            {
                TempData["error"] = "Delete Error, Company Not Found";
                return NotFound(new { message = "Category not found" });
            }

            _unitOfWork.Company.Remove(catagoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Company Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}

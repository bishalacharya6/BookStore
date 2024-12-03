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
    public class CatagoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatagoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Catagory> objCatagory = _unitOfWork.Catagory.GetAll().ToList();
            return View(objCatagory);
        }

        public IActionResult CreateCatagory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCatagory(Catagory obj)
        {
            if (obj.Name.Equals(obj.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("name", "Name and Display Order cannot be the same");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Catagory.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Catagory Created Successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult EditCatagory(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Edit Error Occurred. Catagory Not Found!";
                return NotFound();
            }

            var catagoryFromDb = _unitOfWork.Catagory.Get(c => c.Id == id);

            if (catagoryFromDb == null)
            {
                return NotFound();
            }

            return View(catagoryFromDb);
        }

        [HttpPost]
        public IActionResult EditCatagory(Catagory obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Catagory.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Catagory Updated Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Edit Error Occurred. Failed!";
            return View(obj);
        }

        [HttpPost]
        public IActionResult DeleteCatagory(int id)
        {
            var catagoryFromDb = _unitOfWork.Catagory.Get(c => c.Id == id);

            if (catagoryFromDb == null)
            {
                TempData["error"] = "Delete Error, Catagory Not Found";
                return NotFound(new { message = "Category not found" });
            }

            _unitOfWork.Catagory.Remove(catagoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Catagory Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}

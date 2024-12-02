using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Collections.Generic;
using Web.DataAccess;
using Web.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models.ViewModels;
using System.IO.Enumeration;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties: "Catagory").ToList();

            return View(objProduct);
        }

        public IActionResult UpsertProduct(int? id)
        {
/*            IEnumerable<SelectListItem> CatagoryList = _unitOfWork.Catagory.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });*/
            // DIFFERENT TYPES OF DATA TRANSFERS (viewbag, viewdata, and tempdata for shorter transfer of data.)
            // ViewBag.CatagoryList = CatagoryList;
            // ViewData["CatagoryList"] = CatagoryList;


            ProductViewModel productVm = new()
            {
                CatagoryList = _unitOfWork.Catagory.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                // For create 
                return View(productVm);
            }
            else
            {
                // For Update/edit
                productVm.Product = _unitOfWork.Product.Get(a => a.Id == id);
                return View(productVm);
            }


        }

        [HttpPost]
        public IActionResult UpsertProduct(ProductViewModel obj, IFormFile? formFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (formFile != null) { 
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // Delete Old Image File
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                     
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(obj.Product.Id ==0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CatagoryList = _unitOfWork.Catagory.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                return View(obj);
            }
        }

      /*  public IActionResult EditProduct(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Edit Error Occurred. Product Not Found!";
                return NotFound();
            }

            var productFromDb = _unitOfWork.Product.Get(c => c.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult EditProduct(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Edit Error Occurred. Failed!";
            return View(obj);
        }

*/

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var productFromDb = _unitOfWork.Product.Get(c => c.Id == id);

            if (productFromDb == null)
            {
                TempData["error"] = "Delete Error, Product Not Found";
                return NotFound(new { message = "Category not found" });
            }

            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}

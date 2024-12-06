using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Web.DataAccess.Repository;
using Web.Models;

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Catagory");
            return View(productList);
        } 
        
        public IActionResult ProductDetails(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Catagory"),
                Count = 1,
                ProductId = productId
            };
            
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProductDetails(ShoppingCart cart)
        {
            // Getting the Id of the user Logged In. 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=> u.ApplicationUserId == userId && u.ProductId == cart.ProductId);

            if(cartFromDb != null)
            {
                // Update Entry as the item was already in the cart
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                // New Entry as the item was not in the cart
                _unitOfWork.ShoppingCart.Add(cart);
            }

            _unitOfWork.Save();

            

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

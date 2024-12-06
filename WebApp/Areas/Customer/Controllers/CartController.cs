using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.DataAccess;
using Web.DataAccess.Repository;
using Web.Models;
using Web.Models.ViewModels;

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewModel shoppingCartViewModel { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product")
            };

            // Using LINQ, cleaner and readable. 
            shoppingCartViewModel.OrderTotal = shoppingCartViewModel.ShoppingCartList.Sum(cart =>
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                return cart.Count * cart.Price;
            });

            // Same thing using foreach loop
            /*foreach(var cart in shoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartViewModel.OrderTotal += cart.Count * cart.Price;
            }*/

            return View(shoppingCartViewModel);
        }
        
        public IActionResult Summary()
        {
            return View("Summary");
        }



        public IActionResult Plus(int cartId)
        {
            var cartItemDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartItemDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartItemDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartItemDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartItemDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartItemDb);
            }
            else
            {

                cartItemDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartItemDb);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int cartId)
        {
            var cartItemDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartItemDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }



        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }

        }
    }
}

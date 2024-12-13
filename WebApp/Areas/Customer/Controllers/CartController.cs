using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Numerics;
using System.Security.Claims;
using Web.DataAccess;
using Web.DataAccess.Repository;
using Web.Models;
using Web.Models.ViewModels;
using Web.Utility;

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            // Using LINQ, cleaner and readable. 
            shoppingCartViewModel.OrderHeader.OrderTotal = shoppingCartViewModel.ShoppingCartList.Sum(cart =>
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

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            shoppingCartViewModel.OrderHeader.OrderTotal = shoppingCartViewModel.ShoppingCartList.Sum(cart =>
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                return cart.Count * cart.Price;
            });

            return View(shoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            shoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

            // this is right way 
            ApplicationUsers applicationUsers = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // this is wrong way
            // shoppingCartViewModel.OrderHeader.ApplicationUsers = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            shoppingCartViewModel.OrderHeader.OrderTotal = shoppingCartViewModel.ShoppingCartList.Sum(cart =>
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                return cart.Count * cart.Price;
            });

            // To check if Order is made by normal user or company user
            if (applicationUsers.CompanyId.GetValueOrDefault() == 0)
            {
                // Normal User
                shoppingCartViewModel.OrderHeader.PaymentStatus = PaymenetManagement.PaymentStatusPending;
                shoppingCartViewModel.OrderHeader.OrderStatus = PaymenetManagement.StatusPending;
            }
            else
            {
                // Company User
                shoppingCartViewModel.OrderHeader.PaymentStatus = PaymenetManagement.PaymentStatusDelayedPayment;
                shoppingCartViewModel.OrderHeader.OrderStatus = PaymenetManagement.StatusPending;

            }


            _unitOfWork.OrderHeader.Add(shoppingCartViewModel.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartViewModel.ShoppingCartList)
            {
                OrderDetail detail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartViewModel.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(detail);
                _unitOfWork.Save();
            }

            if (applicationUsers.CompanyId.GetValueOrDefault() == 0) 
            {
                var DOMAIN = "http://localhost:5073";
				var successDomain = DOMAIN + $"/customer/cart/OrderConfirmation?id={shoppingCartViewModel.OrderHeader.Id}";
                var cancelDomain = DOMAIN + $"/customer/cart/index";
                    
				var options = new SessionCreateOptions
				{
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = successDomain,
					CancelUrl = cancelDomain,
				};

                foreach(var item in shoppingCartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //  $20.50 = 2050
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }   

				var service = new SessionService();
				Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentStatus(shoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

			}


			return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartViewModel.OrderHeader.Id });
			


		}

        public IActionResult OrderConfirmation (int id )
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u =>  u.Id == id, includeProperties: "ApplicationUsers");
            if(orderHeader.PaymentStatus != PaymenetManagement.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentStatus(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, session.Id, PaymenetManagement.PaymentStatusApproved);
                    _unitOfWork.Save();

                }
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();


            return View(id);
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

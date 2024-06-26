﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace BookUI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();

            using var transcation = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("user is not logged-in");
                }
                var cart = await GetCart(userId);

                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges(); // save cart info 

                // cart detail
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Bookks.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };

                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transcation.Commit();
            }
            catch (Exception ex) {}
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int bookId)
        {
            //using var transcation = _db.Database.BeginTransaction();
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("user is not logged in");
                }

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new Exception("invalid cart");
                }

                // cart detail
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                {
                    throw new Exception("No items in cart");
                }

                else if (cartItem.Quantity == 1) // remove it from cart 
                {
                    _db.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity--;
                }
                _db.SaveChanges();

            }
            catch (Exception ex) {}
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
            {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new Exception("Invalid userid");
            }
            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a=>a.Book)
                .ThenInclude(a=>a.Genre)
                .Where(a=>a.UserId==userId).FirstOrDefaultAsync();
            return  shoppingCart;
            }

        public async Task< ShoppingCart> GetCart(string userId) 
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }
        public async Task<int> GetCartItemCount(string userId ="")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join CartDetail in _db.CartDetails
                              on cart.Id equals CartDetail.ShoppingCartId
                              where cart.UserId==userId
                              select new {CartDetail.Id}
                              ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckOut() {
            using var transaction = _db.Database.BeginTransaction();
            try 
            {
                // logic
                // move data from cartDetail to order // order detail then remove cart detail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                
                    throw new Exception("User is not logged-in");
                    var cart = await GetCart(userId);

                    if(cart is null) 
                     throw new Exception("Invalid cart");
                var cartDetail = _db.CartDetails
                                  .Where(a => a.ShoppingCartId == cart.Id).ToList();
                   
                if (cartDetail.Count == 0)
                        throw new Exception("Cart is empty");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId =  1 // Pending.

                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach(var i in cartDetail)
                {
                    var orderDetail = new OrderDetail {

                        BookId = i.BookId,
                        OrderId = order.Id,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
                //removing the cart details.
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ) {
                return false;
            }
        }
        private string GetUserId() // get user id
        {
            var principal = _httpContextAccessor.HttpContext.User; // ClaimsPrincipal
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}

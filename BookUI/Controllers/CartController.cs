﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            this._cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int bookId,int qty=1,int redirect=0)
        {
            var cartCount = await _cartRepo.AddItem(bookId,qty);
            if (redirect == 0) 
                return Ok(cartCount);
                return RedirectToAction ("GetUserCart");   
        }

        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await _cartRepo.RemoveItem(bookId);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount ();
            return Ok(cartItem);
        }      
        
        public async Task<IActionResult> Checkout()
        {
            bool isCheckdOut = await _cartRepo.DoCheckOut();
            if (!isCheckdOut)
                throw new Exception("problem in server side");
            return RedirectToAction("Index","Home");
        }
    }
}

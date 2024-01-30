using BookUI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookUI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public  DbSet<Book> Bookks { get; set; } 
        // add table in database named Books but in class book named Book
        // table name will be Book cuz we defined it in class book
        public  DbSet<Genre> Genres { get; set; } 
        public  DbSet<ShoppingCart> ShoppingCarts { get; set; } 
        public  DbSet<CartDetail> CartDetaila { get; set; } 
        public  DbSet<Order> Orders { get; set; } 
        public  DbSet<OrderDetail> OrderDetails { get; set; } 
        public  DbSet<OrderStatus> OrderStatus { get; set; } 
    }
}

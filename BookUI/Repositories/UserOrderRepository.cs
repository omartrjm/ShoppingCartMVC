using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookUI.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public UserOrderRepository(ApplicationDbContext db,IHttpContextAccessor httpContextAccessor,UserManager<IdentityUser> userManger)
        {
            this._db = db;
            this._httpContextAccessor = httpContextAccessor;
            this._userManager = userManger;
        }
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");

            var orders = await _db.Orders
                .Include(x=> x.OrderStatus)
                .Include(x=>x.OrderDetail)
                .ThenInclude(x=>x.Book)
                .ThenInclude(x=> x.Genre)
                .Where(a=>a.UserId==userId)
                .ToListAsync();
            return orders;
        }
        private string GetUserId() // get user id
        {
            var principal = _httpContextAccessor.HttpContext.User; // ClaimsPrincipal
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}

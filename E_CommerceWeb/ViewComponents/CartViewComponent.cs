using Microsoft.AspNetCore.Mvc;
using E_CommerceWeb.ViewModels;
using E_CommerceWeb.Data;
using E_CommerceWeb.Helper;

namespace E_CommerceWeb.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly EcommerceDbContext _context;

        public CartViewComponent(EcommerceDbContext context) => _context = context;

        public IViewComponentResult Invoke()
        {
            var gioHang = HttpContext.Session.Get<List<GioHangVM>>(MySettings.CartKey) ?? new List<GioHangVM>();
            return View("CartPanel",new GioHangPanel
            {
                SoLuong = gioHang.Sum(p => p.SoLuong),
                ThanhTien = gioHang.Sum(p => p.ThanhTien)
            }); 
        }
    }
}

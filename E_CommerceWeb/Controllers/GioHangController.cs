using E_CommerceWeb.Data;
using E_CommerceWeb.Helper;
using E_CommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWeb.Controllers
{
    public class GioHangController : Controller
    {
        private readonly EcommerceDbContext _context;

        public GioHangController(EcommerceDbContext context) => _context = context;

        public List<GioHangVM> GioHang => HttpContext.Session.Get<List<GioHangVM>>(MySetttings.CartKey) ?? new List<GioHangVM>() ;
        public IActionResult Index()
        {
            return View(GioHang);
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = GioHang;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                    return Redirect("/404");
                }
                item = new GioHangVM
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    Hinh = hangHoa.Hinh ?? "",
                    DonGia = hangHoa.DonGia ?? 0,
                    SoLuong = quantity

                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySetttings.CartKey, gioHang);
            return RedirectToAction("Index");
        }
        
      
        public IActionResult RemoveFromCart(int id)
        {
            var gioHang = GioHang;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {                
                gioHang.Remove(item);
            }            
            HttpContext.Session.Set(MySetttings.CartKey, gioHang);
            return RedirectToAction("Index");
        }
    
    }
}

using E_CommerceWeb.Data;
using E_CommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWeb.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly EcommerceDbContext _context;

        public MenuLoaiViewComponent(EcommerceDbContext context) => _context = context;

        public IViewComponentResult Invoke()
        {
            var data = _context.Loais.Select(p => new LoaiVM
            {
                MaLoai = p.MaLoai,
                TenLoai = p.TenLoai,
                SoLuong = p.HangHoas.Count 
            });
            return View(data);
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWeb.Controllers
{
    public class HangHoaController : Controller
    {
        public IActionResult Index(int? MaLoai)
        {
            return View();
        }
    }
}

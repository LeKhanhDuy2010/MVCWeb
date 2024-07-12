using AutoMapper;
using E_CommerceWeb.Data;
using E_CommerceWeb.Helper;
using E_CommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_CommerceWeb.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public KhachHangController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
     
        #region Đăng ký
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(DangKyThanhVienVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtils.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;//sẽ xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtils.UploadHinh(Hinh, "KhachHang");
                    }

                    _context.Add(khachHang);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }
            return View();
        }
        #endregion Kết thúc đăng ký
        #region Đăng nhập
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapVM model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if(ModelState.IsValid)
            {
                var khachHang = _context.KhachHangs.SingleOrDefault(p => p.MaKh == model.UserName);
                if(khachHang == null)
                {
                    ModelState.AddModelError("loi", "Khách hàng không tồn tại!");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản bị khóa. Liên hệ admin để được hỗ trợ!");
                    }
                    else
                    {
                        if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập, vui lòng nhập lại!");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySettings.CLAIM_CUSTOMERID, khachHang.MaKh),
                                new Claim(ClaimTypes.Role, "Customer"),
                            };
                            var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            
                        }
                    }
                }
            }
            return View();
        }

        #endregion
        [Authorize]

        public IActionResult Profile()
        {

            // Lấy tên khách hàng từ claims
            var hoTen = User.FindFirst(ClaimTypes.Name)?.Value;

            // Truyền thông tin này đến view
            var khachHang = new KhachHangVM
            {
                TenKh = hoTen
            };
            return View(khachHang);
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}

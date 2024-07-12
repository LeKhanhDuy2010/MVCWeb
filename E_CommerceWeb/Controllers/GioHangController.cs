using E_CommerceWeb.Data;
using E_CommerceWeb.Helper;
using E_CommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWeb.Controllers
{
    public class GioHangController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly PaypalClient _paypalClient;

        public GioHangController(EcommerceDbContext context, PaypalClient paypalClient) 
        {
            _context = context;
            _paypalClient = paypalClient;
        }

        public List<GioHangVM> GioHang => HttpContext.Session.Get<List<GioHangVM>>(MySettings.CartKey) ?? new List<GioHangVM>() ;
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
            HttpContext.Session.Set(MySettings.CartKey, gioHang);
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
            HttpContext.Session.Set(MySettings.CartKey, gioHang);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if(GioHang.Count == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientdId = _paypalClient.ClientId;
            return View(GioHang);
        }

		[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutVM model)
		{
			if (ModelState.IsValid)
			{
				var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySettings.CLAIM_CUSTOMERID).Value;
				var khachHang = new KhachHang();
				if (model.GiongKhachHang)
				{
					khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
				}

				var hoadon = new HoaDon
				{
					MaKh = customerId,
					HoTen = model.HoTen ?? khachHang.HoTen,
					DiaChi = model.DiaChi ?? khachHang.DiaChi,
					DienThoai = model.DienThoai ?? khachHang.DienThoai,
					NgayDat = DateTime.Now,
					CachThanhToan = "COD",
					CachVanChuyen = "GRAB",
					MaTrangThai = 0,
					GhiChu = model.GhiChu
				};

				_context.Database.BeginTransaction();
				try
				{
					_context.Database.CommitTransaction();
					_context.Add(hoadon);
					_context.SaveChanges();

					var cthds = new List<ChiTietHd>();
					foreach (var item in GioHang)
					{
						cthds.Add(new ChiTietHd
						{
							MaHd = hoadon.MaHd,
							SoLuong = item.SoLuong,
							DonGia = item.DonGia,
							MaHh = item.MaHh,
							GiamGia = 0
						});
					}
					_context.AddRange(cthds);
					_context.SaveChanges();

					HttpContext.Session.Set<List<GioHangVM>>(MySettings.CartKey, new List<GioHangVM>());

					return View("Success");
				}
				catch
				{
					_context.Database.RollbackTransaction();
				}
			}

			return View(GioHang);
		}



        #region Paypal payment
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        [HttpPost("/GioHang/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Thông tin đơn hàng gửi qua Paypal
            var tongTien = GioHang.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/GioHang/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // Lưu database đơn hàng của mình

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion

    }
}

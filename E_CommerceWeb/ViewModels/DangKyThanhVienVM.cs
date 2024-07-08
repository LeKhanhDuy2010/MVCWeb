using System.ComponentModel.DataAnnotations;

namespace E_CommerceWeb.ViewModels
{
    public class DangKyThanhVienVM
    {
        [Key]
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string HoTen {  get; set; }

        [Display(Name = "Giới tính")]        
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        public string DiaChi { get; set; }

        [Display(Name="Điện thoại")]
        [MaxLength(24,ErrorMessage ="Tối đa 24 kí tự")]
        [RegularExpression(@"0[9876]\d{8}",ErrorMessage ="Chưa đúng định dạng di động Việt Nam")]
        public string DienThoai { get; set; }

        [Display(Name = "Email")]
        [MaxLength(60, ErrorMessage = "Chưa đúng định dạng Email")]
        public string Email { get; set; }

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }

    }
}

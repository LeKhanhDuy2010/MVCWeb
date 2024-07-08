using System.ComponentModel.DataAnnotations;

namespace E_CommerceWeb.ViewModels
{
    public class DangNhapVM
    {
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="Chưa nhập username")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string UserName { get; set; }

        [Display(Name ="Mật khẩu")]
        [Required(ErrorMessage ="Chưa nhập password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

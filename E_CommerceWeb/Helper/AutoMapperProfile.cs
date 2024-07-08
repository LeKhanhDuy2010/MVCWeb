using AutoMapper;
using E_CommerceWeb.Data;
using E_CommerceWeb.ViewModels;

namespace E_CommerceWeb.Helper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DangKyThanhVienVM,KhachHang>();
        }
    }
}

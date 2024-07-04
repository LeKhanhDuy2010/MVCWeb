namespace E_CommerceWeb.ViewModels
{
    public class GioHangVM
    {
        public int MaHh { get; set; }
        public string Hinh { get; set; }
        public string TenHh { get; set;}
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public double ThanhTien => SoLuong * DonGia;
    }
}

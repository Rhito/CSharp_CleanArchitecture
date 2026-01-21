using System;
using System.Collections.Generic;
using System.Text;
using LearnC_.Interface;

namespace LearnC_.OOP
{
    public class DienThoai : SanPham, IGiaoHoaToc, IBaoHanh
    {
        public decimal GiaGoc;
        public decimal PhiVanChuyen;
        public decimal PhiGiaoHang { get; set; } = 100000m;
        public string UocTinhThoiGianGiaoHang() => "Giao trong 2 giờ.";

        public int ThoiGianBaoHanh { get; set; } = 12;

        public DienThoai(string ten, decimal mucGiamGia, int soLuong, decimal giaGoc, decimal phiVanChuyen ) : base(ten, mucGiamGia, soLuong)
        {
            GiaGoc = giaGoc;
            PhiVanChuyen = phiVanChuyen;
        }

        public override decimal TinhGiaBan()
        {
           return GiaGoc + PhiVanChuyen - MucGiamGia;
        }
         
        public override void BanHang(int soLuongCanMua)
        {
            
            if(SoLuong >= soLuongCanMua)
            {
                Console.WriteLine("⚠️ Nhắc nhở: Vui lòng kiểm tra seal và dán tem bảo hành cho khách!");
            }
            base.BanHang(soLuongCanMua);
        }

        public string LayThongTinBaoHanh()
        {
            return $"Sản phẩm '{Ten}' được bảo hành chính hãng {ThoiGianBaoHanh} tháng.";
        }
    }
}

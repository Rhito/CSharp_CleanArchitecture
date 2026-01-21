using LearnC_.Interface;
using System;


namespace LearnC_.OOP
{
    public class PhanMemPremium : PhanMem, IBaoHanh
    {
        public int ThoiGianBaoHanh { get; set; } = 24;
        public PhanMemPremium(string ten, decimal mucGiamGia, int soLuong, decimal giaGoc) : base(ten, mucGiamGia, soLuong, giaGoc)
        {
            
        }

        public string LayThongTinBaoHanh()
        {
            return $"Phần mềm '{Ten}' được bảo hành chính hãng {ThoiGianBaoHanh} tháng.";
        }


    }
}

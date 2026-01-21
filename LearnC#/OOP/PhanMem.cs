using System;
using System.Collections.Generic;
using System.Text;

namespace LearnC_.OOP
{
    public class PhanMem : SanPham
    {
        public decimal GiaBanQuyen;

        public PhanMem(string ten,decimal mucGiamGia, int soLuong,decimal giaBanQuyen) : base(ten, mucGiamGia, soLuong)
        {
            GiaBanQuyen = giaBanQuyen;
        }

        public override decimal TinhGiaBan()
        {
            return GiaBanQuyen * (1 - MucGiamGia/100);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LearnC_.OOP
{
    public abstract class SanPham
    {
        private string _ten;
        private int _soLuong;
        public int SoLuong
        {
            get { return _soLuong; }
        }
        public string Ten
        {
            get { return _ten; }
        }
        public decimal MucGiamGia { get; set; }
        public SanPham(string ten, decimal mucGiamGia, int soLuong)
        {
            _ten = ten;
            MucGiamGia = mucGiamGia;
            _soLuong = soLuong;
        }

        public abstract decimal TinhGiaBan();

        public virtual void BanHang(int soLuongCanMua)
        {
            if (_soLuong < soLuongCanMua)
            {
                Console.WriteLine($"Số lượng sản phẩm '{_ten}' không đủ để bán. Hiện có: {_soLuong}, Yêu cầu: {soLuongCanMua}");
            }
            else
            {
                _soLuong -= soLuongCanMua;
                Console.WriteLine($"Đã bán {soLuongCanMua} sản phẩm '{_ten}'. Số lượng còn lại: {_soLuong}");
            }
        }

    }
}

namespace LearnC_
{
    using LearnC_.Interface;
    using LearnC_.OOP;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            List<SanPham> lstSp =
            [// them so luong 
                new DienThoai("iPhone 15", 500000m , 15, 20000000m, 50000m),
                new DienThoai("Sung Sam",200000m, 20,15000000m, 45000m),
                new PhanMem("Web SSCL", 20m, 15, 50000000m),
                new PhanMem("Web Landing Page", 10m, 12,1000000m),
                new PhanMem("Native android", 12m, 8, 3000000m),
                new PhanMem("Free tier service", 0m, 11, 300000m),
                new PhanMemPremium("High tier service", 5m, 11, 9000000m),
            ];

            //TinhTongTien(lstSp);

            //LocSanPhamTheoGia(lstSp, ">", 5000000m);

            //LocSanPhamHoaToc(lstSp);

            //LocSanPhamDacBiet(lstSp);

            //TinhTongSanPhamLinQ(lstSp);

            //TongSoluongSanPham(lstSp);

            // Biến đổi danh sách sản phẩm thành danh sách chuỗi CSV
            //List<string> dsDongCSV = lstSp.Select(p => $"{p.Ten},{p.TinhGiaBan()},{p.SoLuong}").ToList();
            //File.WriteAllLines("Danh sách sản phẩm", dsDongCSV);
            //Console.WriteLine(Path.GetFullPath("Danh sách sản phẩm.csv"));

            //Console.WriteLine($"{lstSp.FirstOrDefault(q=>q.TinhGiaBan() > 5000000m)?.Ten}");


            //var nhomTheoGia = lstSp.GroupBy(p => p.TinhGiaBan() > 10000000 ? "Cao cấp" : "Bình dân");

            //foreach (var nhom in nhomTheoGia)
            //{
            //    Console.WriteLine($"--- Nhóm: {nhom.Key} ---");
            //    foreach (var sp in nhom)
            //    {
            //        Console.WriteLine($"- {sp.Ten}: {sp.TinhGiaBan():N0}");
            //    }
            //}
            //var baoCaoTonKho = lstSp
            //    .GroupBy(p => p.TinhGiaBan() > 10_000_000)
            //    .Select(g => new
            //    {
            //        LoaiSp = g.Key,
            //        SanPhams = g.ToList(),
            //        TongTon = g.Sum(p => p.SoLuong)
            //    });

            //foreach (var nhom in baoCaoTonKho)
            //{
            //    Console.WriteLine($"--- Nhóm: {nhom.LoaiSp} ---");

            //    foreach (var sp in nhom.SanPhams)
            //    {
            //        Console.WriteLine($"- {sp.Ten}: {sp.TinhGiaBan():N0}");
            //    }

            //    Console.WriteLine($"Tổng tồn: {nhom.TongTon}");
            //}


            var 


        }

        public static void TongSoluongSanPham(List<SanPham> lstSp)
        {
            int tongSoLuongSanPham = lstSp.Sum(p => p.SoLuong);
            Console.WriteLine($"Số lượng sản phẩm: {tongSoLuongSanPham}");
        }
        public static void TinhTrungBinhSanPhamLinQ(List<SanPham> lstSp)
        {
            decimal trungBinhSp = lstSp.Average(p => p.TinhGiaBan());
            Console.WriteLine($"Trung bình giá trị kho hàng: {trungBinhSp:N0} VND");
        }
        public static void TinhTongSanPhamLinQ(List<SanPham> lstSp)
        {
            decimal tongGiaTri = lstSp.Sum(p => p.TinhGiaBan());
            Console.WriteLine($"Tổng giá trị kho hàng: {tongGiaTri:N0} VND");
        }

        public static void LayDanhSachSanPhamLinQ(List<SanPham> lstSp, decimal above)
        {
            lstSp.Where(p => p.TinhGiaBan() > above).Select(p => p.Ten).ToList();
        }

        public static void TinhTongTien(List<SanPham> lstSp)
        {
            decimal tongTien = 0m;
            foreach (SanPham sp in lstSp)
            {
                tongTien += sp.TinhGiaBan();
            }
            Console.WriteLine($"Tổng tiền: {tongTien}");
        }

        public static void LocSanPhamTheoGia(List<SanPham> lstSp, string condition, decimal giaToiThieu)
        {
            foreach (SanPham sp in lstSp)
            {
                decimal giaHienTai = sp.TinhGiaBan(); // Tính một lần để dùng nhiều lần ⚡
                bool thoaDieuKien = false;

                switch (condition)
                {
                    case ">": thoaDieuKien = giaHienTai > giaToiThieu; break;
                    case "<": thoaDieuKien = giaHienTai < giaToiThieu; break;
                    case ">=": thoaDieuKien = giaHienTai >= giaToiThieu; break;
                    case "<=": thoaDieuKien = giaHienTai <= giaToiThieu; break;
                    default:
                        Console.WriteLine($"Điều kiện '{condition}' không hợp lệ!");
                        return; // Thoát hàm nếu điều kiện sai
                }

                if (thoaDieuKien)
                {
                    Console.WriteLine($"{sp.Ten?.PadRight(20)} | Giá: {giaHienTai:N0} VND");
                }
            }
        }

        public static void LocSanPhamHoaToc(List<SanPham> lstSp)
        {
            foreach (SanPham sp in lstSp)
            {
                if (sp is IGiaoHoaToc sanPhamHoaToc)
                {
                    Console.WriteLine($"{sp.Ten} - Giá: {sp.TinhGiaBan():N0} VND - {sanPhamHoaToc.UocTinhThoiGianGiaoHang()}");
                }
            }
        }

        public static void LocSanPhamBaoHanhVaHoaToc(List<SanPham> lstSp)
        {
            Console.WriteLine("--- DANH SÁCH SẢN PHẨM HỖ TRỢ CẢ GIAO HỎA TỐC & BẢO HÀNH ---");
            foreach (var sp in lstSp)
            {
                if (sp is IGiaoHoaToc ght && sp is IBaoHanh bh)
                {
                    Console.WriteLine($"+ {sp.Ten}");
                    Console.WriteLine($"  - Vận chuyển: {ght.UocTinhThoiGianGiaoHang()}");
                    Console.WriteLine($"  - Bảo hành: {bh.LayThongTinBaoHanh()}");
                }
            }
        }

        public static void LocSanPhamDacBiet(List<SanPham> lstSp)
        {
            foreach (SanPham sp in lstSp)
            {
                if (!(sp is IGiaoHoaToc ght) && sp is IBaoHanh bh)
                {
                    Console.WriteLine($"+ {sp.Ten}");
                    Console.WriteLine($"  - Bảo hành: {bh.LayThongTinBaoHanh()}");
                }
            }
        }

    }
}
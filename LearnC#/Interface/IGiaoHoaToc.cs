using System;
using System.Collections.Generic;
using System.Text;

namespace LearnC_.Interface
{
    public interface IGiaoHoaToc
    {
        decimal PhiGiaoHang { get; set; }
        string UocTinhThoiGianGiaoHang();
    }
}

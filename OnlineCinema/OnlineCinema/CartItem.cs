using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCinema.Models
{
    public class CartItem
    {
        public int MaVe { get; set; }
        public int MaLichChieu { get; set; }
        public string TenPhim { get; set; }
        public string TenPhong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public decimal GiaVe { get; set; }
        public int SoLuongVe { get; set; }
        public string MaGhe { get; set; }
        public decimal TongTien => GiaVe * SoLuongVe; // Tính toán tự động
    }
}
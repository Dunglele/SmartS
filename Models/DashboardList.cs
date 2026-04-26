using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarts_DoAn_Backup_27_11_2025.Models
{
    public class DashboardList
    {
        public IEnumerable<SANPHAM> SanPhams { get; set; }
        public IEnumerable<CHITIETHOADON> ChiTietHoaDons { get; set; }
        public IEnumerable<HOADON> HoaDons { get; set; }
        public IEnumerable<NGUOIDUNG> NguoiDungs { get; set; }
    }
}
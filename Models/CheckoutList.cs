using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarts_DoAn_Backup_27_11_2025.Models
{
    public class CheckoutList
    {
        public IEnumerable<DATHANG> DatHangs { get; set; }
        public IEnumerable<HOADON> HoaDons { get; set; }
        public IEnumerable<CHITIETHOADON> ChiTietHoaDons { get; set; }
    }
}
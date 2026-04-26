using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarts_DoAn_Backup_27_11_2025.Models
{
    public class DetailOrders
    {
        public IEnumerable<DATHANG> DatHangs { get; set; }
        public IEnumerable<HOADON> HoaDons {get; set;}
        public IEnumerable<CHITIETHOADON> ChiTiets { get; set; }
    }
}
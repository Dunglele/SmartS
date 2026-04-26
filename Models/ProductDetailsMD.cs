using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarts_DoAn_Backup_27_11_2025.Models
{
    public class ProductDetailsMD
    {
        public Models.SANPHAM Product { get; set; }
        public IEnumerable<SANPHAM> Products { get; set; }
    }
}
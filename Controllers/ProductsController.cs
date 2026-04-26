using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup.Controllers
{
    public class ProductsController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        // GET: Products
        public ActionResult ProductList(string category, string brand, string TimKiems, int? khoanggia)
        {
            // --- GỌI FUNCTION

            if(TimKiems != null)
            {
                if(TimKiems == "none")
                {
                    return View(db.SANPHAM.ToList());
                }
                return View(db.SANPHAM.Where(x => x.TENSP.Contains(TimKiems)).ToList());
            }

            if(khoanggia.HasValue)
            {
                switch(khoanggia)
                {
                    case 0:
                        return View(db.SANPHAM.ToList());
                    case 1:
                        return View(db.SANPHAM.Where(x => x.GIA < 100000));
                    case 2:
                        return View(db.SANPHAM.Where(x => x.GIA >= 100000 && x.GIA <= 1000000));
                    case 3:
                        return View(db.SANPHAM.Where(x => x.GIA > 1000000));
                }
            }

            var listSp = db.SANPHAM.OrderBy(x => Guid.NewGuid()).ToList();

            if (category != null && brand != null)
            {
                return View(listSp.Where(x => x.MADM == category && x.THUONGHIEU == brand).ToList());
            }

            if (category != null && brand == null)
            {
                return View(listSp.Where(x => x.MADM == category).ToList());
            }

            return View(listSp);
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                //RedirectToAction("Error","Shared");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM SANPHAM = db.SANPHAM.Find(id);
            if (SANPHAM == null)
            {
                return HttpNotFound();
            }

            var listSp = db.SANPHAM.Include("DANHMUC");
            ViewBag.ListSP = listSp;

            return View(new ProductDetailsMD
            {
                Product = SANPHAM,
                Products = listSp
            });
        }

        public ActionResult Filter()
        {
            return PartialView(db.DANHMUC.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
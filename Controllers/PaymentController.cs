using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup.Controllers
{
    public class PaymentController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();

        // GET: Payment/Cart
        public ActionResult Cart()
        {
            try
            {
                if (Session["MAND"] == null)
                {
                    return RedirectToAction("Signin", "Home");
                }

                string mAND = Session["MAND"].ToString();
                var listGioHang = db.GIOHANG.Where(x => x.MAND == mAND).ToList();
                return View(listGioHang);
            }
            catch
            {
                return RedirectToAction("Signin", "Home");
            }
        }

        // POST: Payment/Cart (Thêm vào giỏ hàng)
        [HttpPost]
        public ActionResult Cart(GIOHANG newGH)
        {
            if (Session["MAND"] != null)
            {
                string mAND = Session["MAND"].ToString();
                newGH.MAND = mAND; // Đảm bảo gán đúng MAND từ session

                var check = db.GIOHANG.FirstOrDefault(x => x.MAND == newGH.MAND && x.MASP == newGH.MASP);

                if (check != null)
                {
                    // Nếu sản phẩm đã có, cộng dồn số lượng
                    check.SOLUONG = (check.SOLUONG ?? 0) + (newGH.SOLUONG ?? 1);
                    db.SaveChanges();
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    if (newGH.SOLUONG == null) newGH.SOLUONG = 1;
                    db.GIOHANG.Add(newGH);
                    db.SaveChanges();
                }

                return RedirectToAction("Cart", "Payment");
            }

            return RedirectToAction("Signin", "Home");
        }

        // --- MỚI: Action xử lý cập nhật số lượng qua AJAX ---
        [HttpPost]
        public JsonResult UpdateCartItem(string masp, int soluong)
        {
            try
            {
                string mAND = Session["MAND"]?.ToString();
                if (string.IsNullOrEmpty(mAND))
                {
                    return Json(new { success = false, msg = "Phiên đăng nhập đã hết hạn." });
                }

                // 1. Tìm sản phẩm trong giỏ hàng
                var item = db.GIOHANG.FirstOrDefault(x => x.MASP == masp && x.MAND == mAND);

                if (item != null)
                {
                    // 2. Kiểm tra tồn kho (Optional - Nên có)
                    var product = db.SANPHAM.FirstOrDefault(p => p.MASP == masp);
                    if (product != null && soluong > product.SOLUONG)
                    {
                        return Json(new { success = false, msg = $"Kho chỉ còn {product.SOLUONG} sản phẩm." });
                    }

                    // 3. Cập nhật số lượng
                    item.SOLUONG = soluong;
                    db.SaveChanges();

                    // 4. Tính toán lại các con số để trả về View
                    var cartItems = db.GIOHANG.Where(x => x.MAND == mAND).ToList();

                    // Thành tiền của dòng vừa sửa
                    decimal? itemTotal = (item.GIA ?? 0) * (item.SOLUONG ?? 0);

                    // Tổng tiền cả giỏ
                    decimal subTotal = cartItems.Sum(x => (x.GIA ?? 0) * (x.SOLUONG ?? 0));
                    decimal tax = subTotal * 0.1m; // 10% VAT
                    decimal total = subTotal + tax;

                    // 5. Trả về JSON
                    return Json(new
                    {
                        success = true,
                        itemTotal = itemTotal, // Trả về số chưa format, JS sẽ format
                        subTotal = subTotal,
                        tax = tax,
                        total = total
                    });
                }

                return Json(new { success = false, msg = "Không tìm thấy sản phẩm trong giỏ." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Lỗi server: " + ex.Message });
            }
        }

        // DELETE: Payment/DeleteCart
        public ActionResult DeleteCart(string idn, string idm)
        {
            if (string.IsNullOrEmpty(idm) || string.IsNullOrEmpty(idn))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GIOHANG idDel = db.GIOHANG.FirstOrDefault(x => x.MAND.Trim() == idn.Trim() && x.MASP.Trim() == idm.Trim());
            if (idDel != null)
            {
                db.GIOHANG.Remove(idDel);
                db.SaveChanges();
            }

            return RedirectToAction("Cart");
        }

        // GET: Payment/Checkout
        public ActionResult Checkout()
        {
            if (Session["MAND"] == null) return RedirectToAction("Signin", "Home");

            string mAND = Session["MAND"].ToString();
            var cartItems = db.GIOHANG.Where(x => x.MAND == mAND).ToList();

            ViewBag.CountDH = cartItems.Count;
            return View(cartItems);
        }

        // POST: Payment/Checkout (Logic Transaction của bạn)
        [HttpPost]
        public ActionResult Checkout(DATHANG nDH, HOADON nHD)
        {
            // 1. Kiểm tra đăng nhập
            if (Session["MAND"] == null)
            {
                return RedirectToAction("Signin", "Home");
            }
            var MAND = Session["MAND"].ToString();

            // 2. Lấy giỏ hàng
            var listGH = db.GIOHANG.Where(x => x.MAND == MAND).ToList();
            if (listGH == null || listGH.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Home");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // --- BƯỚC 1: KIỂM TRA TỒN KHO (VALIDATION) ---
                    foreach (var item in listGH)
                    {
                        var product = db.SANPHAM.FirstOrDefault(x => x.MASP == item.MASP);
                        if (product == null || item.SOLUONG > product.SOLUONG)
                        {
                            TempData["Error"] = $"Sản phẩm {item.TENSP} chỉ còn {product?.SOLUONG ?? 0} sản phẩm. Vui lòng cập nhật lại giỏ hàng.";
                            return RedirectToAction("Cart", "Payment");
                        }
                    }

                    // --- BƯỚC 2: TẠO MÃ TỰ ĐỘNG (DATHANG) ---
                    var lastDH = db.DATHANG.OrderByDescending(x => x.MADH).FirstOrDefault();
                    string nextMADH = "DH001";
                    if (lastDH != null)
                    {
                        string numPart = lastDH.MADH.Substring(2);
                        if (int.TryParse(numPart, out int num))
                        {
                            nextMADH = "DH" + (num + 1).ToString("D3");
                        }
                    }
                    nDH.MADH = nextMADH;
                    nDH.MAND = MAND;
                    // Gán thời gian tạo đơn hàng nếu chưa có
                    db.DATHANG.Add(nDH);

                    // --- BƯỚC 3: TẠO MÃ TỰ ĐỘNG (HOADON) ---
                    var lastHD = db.HOADON.OrderByDescending(x => x.MAHD).FirstOrDefault();
                    string nextMAHD = "HD001";
                    if (lastHD != null)
                    {
                        string numPartHD = lastHD.MAHD.Substring(2);
                        if (int.TryParse(numPartHD, out int numHD))
                        {
                            nextMAHD = "HD" + (numHD + 1).ToString("D3");
                        }
                    }

                    nHD.MADH = nextMADH;
                    nHD.MAHD = nextMAHD;
                    nHD.TINHTRANG = "Chờ xử lý";
                    db.HOADON.Add(nHD);

                    // --- BƯỚC 4: XỬ LÝ CHI TIẾT, TRỪ KHO, XÓA GIỎ HÀNG ---
                    foreach (var item in listGH)
                    {
                        // A. Thêm Chi tiết hóa đơn
                        CHITIETHOADON nCTHD = new CHITIETHOADON
                        {
                            MAHD = nextMAHD,
                            MASP = item.MASP,
                            TENSP = item.TENSP,
                            SOLUONG = item.SOLUONG,
                            GIA = item.GIA,
                            TAMTINH = item.TAMTINH,
                            THANHTIEN = (item.GIA ?? 0) * (item.SOLUONG ?? 0)
                        };
                        db.CHITIETHOADON.Add(nCTHD);

                        // B. Trừ tồn kho
                        var productToUpdate = db.SANPHAM.FirstOrDefault(x => x.MASP == item.MASP);
                        if (productToUpdate != null)
                        {
                            productToUpdate.SOLUONG = productToUpdate.SOLUONG - item.SOLUONG;
                        }

                        // C. Xóa khỏi giỏ hàng
                        db.GIOHANG.Remove(item);
                    }

                    // --- BƯỚC 5: LƯU VÀ CAM KẾT ---
                    db.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("Succes", "Payment");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                    return RedirectToAction("Cart", "Payment");
                }
            }
        }

        // Partial View: Đếm số lượng hiển thị trên menu
        public ActionResult CountCart()
        {
            int count = 0;
            if (Session["MAND"] != null)
            {
                string mAND = Session["MAND"].ToString().Trim();
                count = db.GIOHANG.Where(x => x.MAND == mAND).Count();
            }

            ViewBag.Count = count;
            return PartialView();
        }

        public ActionResult Succes()
        {
            return View();
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
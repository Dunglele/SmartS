using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Smarts_DoAn_Backup_27_11_2025.Models;
using System.Threading.Tasks;

namespace Smarts_DoAn_Backup_27_11_2025.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();

        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            var listSP = db.SANPHAM.ToList();
            var listCTHD = db.CHITIETHOADON.Include(c => c.HOADON).Include(c => c.SANPHAM);
            var listHD = db.HOADON.Include(h => h.DATHANG);
            var listND = db.NGUOIDUNG.ToList();

            return View(new DashboardList
            {
                SanPhams = listSP,
                ChiTietHoaDons = listCTHD,
                HoaDons = listHD,
                NguoiDungs = listND
            });
        }

        public ActionResult Orders()
        {
            var listDH = db.DATHANG.ToList();
            var listHD = db.HOADON.Include(h => h.DATHANG);
            var listCTHD = db.CHITIETHOADON.Include(c => c.HOADON).Include(c => c.SANPHAM);

            return View(new CheckoutList
            {
                DatHangs = listDH,
                HoaDons = listHD,
                ChiTietHoaDons = listCTHD
            });
        }
        [HttpPost]
        public ActionResult UpdateOrder(string MADH, string TINHTRANG)
        {
            if (string.IsNullOrEmpty(MADH))
            {
                return RedirectToAction("Orders");
            }
            var hoadon = db.HOADON.FirstOrDefault(h => h.MADH == MADH);

            if (hoadon != null)
            {
                hoadon.TINHTRANG = TINHTRANG;

                db.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái cho đơn " + MADH + " thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy hóa đơn cho mã đặt hàng này!";
            }

            return RedirectToAction("Orders");
        }
        public ActionResult Products()
        {
            var SANPHAM = db.SANPHAM.Include(t => t.DANHMUC);
            return View(SANPHAM.ToList());
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SaveProduct(HttpPostedFileBase fileUpload, Smarts_DoAn_Backup_27_11_2025.Models.SANPHAM model, string Mode)
        {

            string currentImagePath = model.ANHSP;

            // THAM KHẢO NGUỒN TỪ GEMINI
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExt))
                {
                    TempData["Error"] = "Chỉ chấp nhận file ảnh (jpg, png, webp, gif...)";
                    return RedirectToAction("Products");
                }

                try
                {
                    string fileName = System.IO.Path.GetFileName(fileUpload.FileName);
                    string uniqueName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
                    string folderPath = Server.MapPath("~/img_product/product/");

                    if (!System.IO.Directory.Exists(folderPath))
                    {
                        System.IO.Directory.CreateDirectory(folderPath);
                    }

                    string savePath = System.IO.Path.Combine(folderPath, uniqueName);
                    fileUpload.SaveAs(savePath);

                    // Cập nhật đường dẫn mới
                    currentImagePath = "/img_product/product/" + uniqueName;
                    model.ANHSP = currentImagePath;
                }
                catch (Exception)
                {
                    TempData["Error"] = "Lỗi khi lưu ảnh lên server!";
                    return RedirectToAction("Products");
                }
            }

            // Producer them san pham
            try
            {
                if (Mode == "add")
                {

                    var check = db.SANPHAM.Find(model.MASP);
                    if(model.SOLUONG <= 0)
                    {
                        TempData["Error"] = "Thêm sản phẩm không thành công!";
                    }
                    else if (check == null)
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@masp", model.MASP),
                            new SqlParameter("@madm", model.MADM),
                            new SqlParameter("@tensp", model.TENSP),
                            new SqlParameter("@gia", model.GIA),
                            new SqlParameter("@thuonghieu", model.THUONGHIEU),
                            new SqlParameter("@mota", model.MOTA ?? (object)DBNull.Value), // Xử lý giá trị NULL
                            new SqlParameter("@thongso", model.THONGSO ?? (object)DBNull.Value),
                            new SqlParameter("@soluong", model.SOLUONG),
                            new SqlParameter("@anhsp", model.ANHSP ?? (object)DBNull.Value)};

                        //Gọi Stored Procedure bằng ExecuteSqlRawAsync
                        await db.Database.ExecuteSqlCommandAsync(
                            "EXEC SP_THEM_SP @masp, @madm, @tensp, @gia, @thuonghieu, @mota, @thongso, @soluong, @anhsp",
                            parameters
                        );

                        // Lưu ý: Khi gọi Stored Procedure, không cần gọi db.SaveChanges() nữa
                        // vì lệnh EXEC đã thực thi thao tác trực tiếp trên DB.

                        TempData["Message"] = "Thêm sản phẩm thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Mã sản phẩm đã tồn tại!";
                    }
                }
                else if (Mode == "edit")
                {
                    var item = db.SANPHAM.Find(model.MASP);
                    if (item != null)
                    {
                        item.MADM = model.MADM;
                        item.TENSP = model.TENSP;
                        item.GIA = model.GIA;
                        item.THUONGHIEU = model.THUONGHIEU;
                        item.MOTA = model.MOTA;
                        item.THONGSO = model.THONGSO;
                        item.SOLUONG = model.SOLUONG;

                        // Chỉ cập nhật ảnh nếu có upload ảnh mới
                        // Nếu fileUpload == null thì currentImagePath vẫn giữ giá trị cũ (từ hidden field)
                        if (fileUpload != null)
                        {
                            item.ANHSP = currentImagePath;
                        }
                        // Nếu không upload mới, item.ANHSP giữ nguyên trong DB, không cần gán lại

                        db.SaveChanges();
                        TempData["Message"] = "Cập nhật thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Không tìm thấy sản phẩm để sửa!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
                //TempData["Message"] = "Cập nhật thành công!";
            }

            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                SANPHAM idDel = db.SANPHAM.Find(id);
                if (idDel != null)
                {
                    db.SANPHAM.Remove(idDel);
                    db.SaveChanges();
                    TempData["Message"] = "Xóa sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Không thể xóa sản phẩm này vì đã có dữ liệu liên quan (đơn hàng, kho...).";
            }

            return RedirectToAction("Products");
        }
        public ActionResult Categories()
        {
            return View(db.DANHMUC.ToList());
        }
        [HttpPost]
        public async Task<ActionResult> SaveCategory(Smarts_DoAn_Backup_27_11_2025.Models.DANHMUC model, string Mode)
        {
            if (ModelState.IsValid)
            {
                if (Mode == "add")
                {
                    if (db.DANHMUC.FirstOrDefault(v => v.TENDM == model.TENDM) != null)
                    {
                        TempData["Error"] = "Tên danh mục đã tồn tại";
                    }
                    if (model.SOLUONG_SP <= 0)
                    {
                        TempData["Error"] = "Số lượng sản phẩm không được bé hoặc bằng 0";
                    }

                    else if(db.DANHMUC.FirstOrDefault(v => v.TENDM == model.TENDM) == null && model.SOLUONG_SP > 0)
                    {
                        var check = db.DANHMUC.FirstOrDefault(a => a.MADM == model.MADM && a.TENDM == model.TENDM);
                        if (check == null)
                        {

                            var parameters = new[]
                            {
                            new SqlParameter("@madm", model.MADM),
                            new SqlParameter("@tendm", model.TENDM),
                            new SqlParameter("@soluong_sp", model.SOLUONG_SP)
                        };


                            await db.Database.ExecuteSqlCommandAsync(
                                                "EXEC DM_THEM_DM @madm, @tendm, @soluong_sp", parameters);


                        }
                        else
                        {
                            TempData["Error"] = "Mã danh mục trùng";
                        }
                    }
                        
                }
                else if (Mode == "edit")
                {
                    if(db.DANHMUC.FirstOrDefault(v => v.TENDM == model.TENDM) != null)
                    {
                        TempData["Error"] = "Tên danh mục đã tồn tại";
                        return RedirectToAction("Categories");
                    }
                    if(model.SOLUONG_SP <= 0)
                    {
                        TempData["Error"] = "Số lượng sản phẩm không được bé hoặc bằng 0";
                    }
                    // --- XỬ LÝ CẬP NHẬT ---
                    var item = db.DANHMUC.Find(model.MADM);
                    if (item != null)
                    {
                        item.TENDM = model.TENDM;
                        item.SOLUONG_SP = model.SOLUONG_SP;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Categories");
            }

            return RedirectToAction("Categories");
        }
        public ActionResult DeleteCategory(string id)
        {
            if (id.Trim() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DANHMUC idDel = db.DANHMUC.Find(id.Trim());

            if (idDel != null)
            {
                db.DANHMUC.Remove(idDel);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }
        public ActionResult Accounts()
        {
            return View(db.NGUOIDUNG.ToList());
        }
        [HttpPost]
        public async Task<ActionResult> SaveAccount(Smarts_DoAn_Backup_27_11_2025.Models.NGUOIDUNG model, string Mode)
        {
            if (ModelState.IsValid)
            {
                if (Mode == "add")
                {
                    var checkMAND = db.NGUOIDUNG.Find(model.MAND);
                    var checkUSERNAME = db.NGUOIDUNG.Find(model.EMAIL);
                    if (checkMAND == null && checkUSERNAME == null)
                    {
                        var parameters = new[]
                        {
                            new SqlParameter("@mand", model.MAND),
                            new SqlParameter("@email", model.EMAIL),
                            new SqlParameter("@vaitro", model.VAITRO),
                            new SqlParameter("@matkhau", model.MATKHAU),
                            new SqlParameter("@hoten", model.HOTEN),
                            new SqlParameter("@sodienthoai", model.SODIENTHOAI) 
                        };

                        await db.Database.ExecuteSqlCommandAsync(
                                            "EXEC ND_THEM_ND @mand, @email, @vaitro, @matkhau, @hoten, @sodienthoai",parameters);
                    }
                    else
                    {
                        TempData["Error"] = "Vui lòng xem lại thông tin";
                    }
                }
                else if (Mode == "edit")
                {
                    var item = db.NGUOIDUNG.Find(model.MAND);
                    if (item != null)
                    {
                        item.EMAIL = model.EMAIL;
                        item.HOTEN = model.HOTEN;
                        item.VAITRO = model.VAITRO;
                        item.MATKHAU = model.MATKHAU;

                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Accounts"); 
            }

            return RedirectToAction("Accounts");
        }
        public ActionResult DeleteAccount(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NGUOIDUNG idDel = db.NGUOIDUNG.Find(id);

            if (idDel != null)
            {
                db.NGUOIDUNG.Remove(idDel);
                db.SaveChanges();
            }
            return RedirectToAction("Accounts");
        }
        public ActionResult Contact()
        {
            return View(db.LIENHE.ToList());
        }

        [HttpPost]
        public ActionResult DeleteContact(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LIENHE idDel = db.LIENHE.Find(id);

            if (idDel != null)
            {

                db.LIENHE.Remove(idDel);
                db.SaveChanges();
            }
            return RedirectToAction("Contact");
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
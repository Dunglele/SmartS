using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup_27_11_2025.Controllers
{
    public class HomeController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        // GET: TrangChu
        public ActionResult Index()
        {
            var SANPHAM = db.SANPHAM.Include("DANHMUC");
            return View(SANPHAM.OrderBy(x => Guid.NewGuid()).ToList());
        }
        public ActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signin(Smarts_DoAn_Backup_27_11_2025.Models.NGUOIDUNG app)
        {

            var user = db.NGUOIDUNG.FirstOrDefault(us => us.EMAIL == app.EMAIL && us.MATKHAU == app.MATKHAU);
            Session["MAND"] = db.NGUOIDUNG.Where(us => us.EMAIL == app.EMAIL && us.MATKHAU == app.MATKHAU).Select(us => us.MAND).FirstOrDefault();
            Session["HOTEN"] = db.NGUOIDUNG.Where(us => us.EMAIL == app.EMAIL && us.MATKHAU == app.MATKHAU).Select(us => us.HOTEN).FirstOrDefault();
            if (user != null)
            {
                // Tạo vé đăng nhập (Ticket) chứa thông tin User và Quyền (Role)
                // Lưu quyền vào tham số thứ 2 (userData) của ticket
                var ticket = new FormsAuthenticationTicket(
                    1,                                     // Version
                    user.HOTEN,                         // User name
                    DateTime.Now,                          // Ngày tạo
                    DateTime.Now.AddMinutes(30),           // Ngày hết hạn
                    false,                                 // IsPersistent (Ghi nhớ đăng nhập?)
                    user.VAITRO.Trim()                              // <--- LƯU QUYỀN VÀO ĐÂY (VD: "Admin")
                );

                // Mã hóa ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                // Tạo Cookie
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                if (user.VAITRO.Trim() == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                if(user.VAITRO == "Nhân viên")
                {
                    return RedirectToAction("Products", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản mật khẩu";
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(NGUOIDUNG ndung, string XACNHANMATKHAU)
        {
            try
            {
                if (ndung.MATKHAU != XACNHANMATKHAU)
                {
                    ViewBag.Msg = "Mật khẩu và Xác nhận Mật khẩu không khớp.";
                    return View();
                }

                var CEMAIL = db.NGUOIDUNG.FirstOrDefault(x => x.EMAIL == ndung.EMAIL);
                var CSDT = db.NGUOIDUNG.FirstOrDefault(x => x.SODIENTHOAI == ndung.SODIENTHOAI);

                if (CEMAIL != null)
                {
                    ViewBag.Msg = "Email này đã được sử dụng.";
                    return View();
                }

                if (CSDT != null)
                {
                    ViewBag.Msg = "Số điện thoại này đã được sử dụng.";
                    return View();
                }

                var lastND = db.NGUOIDUNG.OrderByDescending(x => x.MAND).FirstOrDefault();
                string MAND;

                if (lastND == null)
                {
                    MAND = "ND001";
                }

                else
                {
                    string lastMa = lastND.MAND;
                    string numberPartString = lastMa.Substring(2);
                    int nextNumber = 1;

                    if (int.TryParse(numberPartString, out int currentNumber))
                    {
                        nextNumber = currentNumber + 1;
                    }

                    string nextNumberString = nextNumber.ToString("D3");
                    MAND = "ND" + nextNumberString;
                }

                ndung.MAND = MAND;
                ndung.VAITRO = "Khách hàng";
                db.NGUOIDUNG.Add(ndung);
                db.SaveChanges();

                return RedirectToAction("Signin", "Home");
            }
            catch(Exception ex)
            {
                ViewBag.Msg = "Đã có lỗi xảy ra, vui lòng liên hệ Admin. Mã lỗi: " + ex;
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session["MAND"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string EMAIL, string HOTEN, string SODIENTHOAI, string DIACHI)
        {
            var newLienHe = new LIENHE
            {
                EMAIL = EMAIL,
                HOTEN = HOTEN,
                SODIENTHOAI = SODIENTHOAI,
                DIACHI = DIACHI
            };
            var lastLienHe = db.LIENHE
                .Select(h => h.MALH)
                .OrderByDescending(malh => malh)
                .FirstOrDefault();

            string MALH = "LH001"; 

            if (lastLienHe != null)
            {
                if (lastLienHe.Length >= 3 && lastLienHe.StartsWith("LH"))
                {
                    string numberPartString = lastLienHe.Substring(2); 

                    if (int.TryParse(numberPartString, out int currentNumber))
                    {
                        int nextNumber = currentNumber + 1;
                        string nextNumberString = nextNumber.ToString("D3");
                        MALH = "LH" + nextNumberString;
                    }
                }
            }
            newLienHe.MALH = MALH;
            db.LIENHE.Add(newLienHe);

            try
            {
                db.SaveChanges();

                TempData["SuccessMessage"] = "Gửi liên hệ thành công!";
                ViewBag.Msg = "Thông tin của bạn đã được lưu lại!!! <3";
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Msg = "Gửi liên hệ thất bại do lỗi hệ thống. Vui lòng thử lại sau.";
                return View();
            }
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult NavTwo()
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
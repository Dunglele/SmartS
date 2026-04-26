using Smarts_DoAn_Backup_27_11_2025.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Smarts_DoAn_Backup_27_11_2025.Controllers
{
    public class UserController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        public ActionResult EditProfile(string id)
        {
            string mand = Session["MAND"].ToString();
            if (id != mand)
            {
                return RedirectToAction("EditProfile", "User", new { id = Session["MAND"].ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOIDUNG nGUOIDUNG = db.NGUOIDUNG.Find(id);
            if (nGUOIDUNG == null)
            {
                return HttpNotFound();
            }
            return View(nGUOIDUNG);
        }

        [HttpPost]
        public ActionResult SaveProfile(string MAND, string HOTEN, string SODIENTHOAI, string DIACHI)
        {
            var mand = db.NGUOIDUNG.FirstOrDefault(us => us.MAND == MAND);
            if (HOTEN == "")
            {
                TempData["Error"] = "Vui lòng nhập họ và tên";
                return RedirectToAction("EditProfile", "User", new { id = MAND });
            }

            if (SODIENTHOAI == "" && SODIENTHOAI != mand.SODIENTHOAI)
            {
                TempData["Error"] = "Số điện thoại không được để trống";
                return RedirectToAction("EditProfile", "User", new { id = MAND });
            }

            var sdt = db.NGUOIDUNG.FirstOrDefault(us => us.SODIENTHOAI == SODIENTHOAI);
            if (SODIENTHOAI != mand.SODIENTHOAI)
            {
                if (sdt != null)
                {
                    TempData["Error"] = "Số điện thoại đã tồn tại, vui lòng kiểm tra lại";
                    return RedirectToAction("EditProfile", "User", new { id = MAND });
                }
            }
            if(mand != null)
            {
                if(mand.HOTEN == HOTEN && mand.SODIENTHOAI == SODIENTHOAI && mand.DIACHI == DIACHI)
                {
                    return RedirectToAction("EditProfile", "User", new { id = MAND });
                }

                mand.HOTEN = HOTEN;
                mand.SODIENTHOAI = SODIENTHOAI;
                mand.DIACHI = DIACHI;
                db.SaveChanges();

                TempData["Success"] = "Cập nhật hồ sơ thành công";
                Session["HOTEN"] = HOTEN;
                return RedirectToAction("EditProfile", "User", new {id = MAND});
            }

            TempData["Error"] = "Đã có lỗi xảy ra, vui lòng xem thử lại hoặc liên hệ với ADMIN";
            return RedirectToAction("EditProfile", "User", new {id = MAND});
        }
        public ActionResult Orders(string id)
        {
            string mand = Session["MAND"].ToString();
            if (id != mand)
            {
                return RedirectToAction("Orders", "User", new { id = Session["MAND"].ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(db.DATHANG.Where(us => us.MAND == id));
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
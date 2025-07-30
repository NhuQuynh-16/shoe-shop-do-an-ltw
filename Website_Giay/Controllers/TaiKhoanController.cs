using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    public class TaiKhoanController : BaseController
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        
        public ActionResult DangNhap()
        {
            // Nếu đã đăng nhập, chuyển hướng về trang phù hợp
            var user = Session["User"] as KhachHang;
            if (user != null)
            {
                if (user.UserRole == 1)
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string tenDangNhap, string matKhau)
        {
            try
            {
                var user = data.KhachHangs.FirstOrDefault(u => u.Username == tenDangNhap && u.UserPassword == matKhau);
                if (user != null)
                {
                    // Lưu thông tin người dùng vào session
                    Session["User"] = user;
                    Session["UserRole"] = user.UserRole;
                    Session["UserId"] = user.IdUser;
                    Session["UserName"] = user.Username;

                    // Kiểm tra returnUrl
                    string returnUrl = TempData["ReturnUrl"] as string;
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Phân quyền điều hướng
                    if (user.UserRole == 1) // Admin
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else // Khách hàng
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
            catch
            {
                ViewBag.ThongBao = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }


        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DangKy()
        {
            // Nếu đã đăng nhập, chuyển về trang chủ
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(KhachHang newUser)
        {
            try
            {
                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrEmpty(newUser.Username))
                {
                    ViewBag.ThongBao = "Vui lòng nhập tên đăng nhập.";
                    return View(newUser);
                }

                if (string.IsNullOrEmpty(newUser.UserPassword))
                {
                    ViewBag.ThongBao = "Vui lòng nhập mật khẩu.";
                    return View(newUser);
                }

                if (string.IsNullOrEmpty(newUser.HoTen))
                {
                    ViewBag.ThongBao = "Vui lòng nhập họ tên.";
                    return View(newUser);
                }

                if (string.IsNullOrEmpty(newUser.Email))
                {
                    ViewBag.ThongBao = "Vui lòng nhập email.";
                    return View(newUser);
                }

                if (string.IsNullOrEmpty(newUser.DienThoai))
                {
                    ViewBag.ThongBao = "Vui lòng nhập số điện thoại.";
                    return View(newUser);
                }

                if (string.IsNullOrEmpty(newUser.DiaChi))
                {
                    ViewBag.ThongBao = "Vui lòng nhập địa chỉ.";
                    return View(newUser);
                }

                // Kiểm tra username đã tồn tại chưa
                var existingUser = data.KhachHangs.FirstOrDefault(u => u.Username == newUser.Username);
                if (existingUser != null)
                {
                    ViewBag.ThongBao = "Tên đăng nhập đã tồn tại.";
                    return View(newUser);
                }

                // Kiểm tra email đã tồn tại chưa
                existingUser = data.KhachHangs.FirstOrDefault(u => u.Email == newUser.Email);
                if (existingUser != null)
                {
                    ViewBag.ThongBao = "Email đã được sử dụng.";
                    return View(newUser);
                }

                // Kiểm tra số điện thoại hợp lệ
                if (!System.Text.RegularExpressions.Regex.IsMatch(newUser.DienThoai, @"^\d{10}$"))
                {
                    ViewBag.ThongBao = "Số điện thoại không hợp lệ. Vui lòng nhập 10 chữ số.";
                    return View(newUser);
                }

                // Mặc định role là khách hàng (0)
                newUser.UserRole = 0;

                // Lưu vào database
                data.KhachHangs.InsertOnSubmit(newUser);
                data.SubmitChanges();

                // Chuyển đến trang đăng nhập
                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Có lỗi xảy ra khi đăng ký: " + ex.Message;
                return View(newUser);
            }
        }

        public ActionResult ThongTinDH()
        {
            var user = Session["User"] as KhachHang;
            if (user == null)
                return RedirectToAction("DangNhap");

            return View(user);
        }

        public ActionResult DonDatHang()
        {
            var user = Session["User"] as KhachHang;
            if (user == null)
                return RedirectToAction("DangNhap");

            var donHang = data.DonHangs.Where(d => d.IdUser == user.IdUser).OrderByDescending(d => d.NgayDatHang).ToList();
            return View(donHang);
        }
    }
}

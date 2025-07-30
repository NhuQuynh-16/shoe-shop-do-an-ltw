using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    //public class AuthorizeAdminAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var user = filterContext.HttpContext.Session["User"] as KhachHang;
    //        if (user == null || user.UserRole != 1)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new System.Web.Routing.RouteValueDictionary(
    //                    new { controller = "TaiKhoan", action = "DangNhap" }));
    //        }
    //        base.OnActionExecuting(filterContext);
    //    }
    //}

    //[AuthorizeAdmin]
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        DataClasses1DataContext data = new DataClasses1DataContext();

        public ActionResult Index()
        {
            // Thống kê tổng quan
            ViewBag.TotalProducts = data.SanPhams.Count();
            ViewBag.TotalCategories = data.DanhMucGiays.Count();
            ViewBag.TotalUsers = data.KhachHangs.Count();
            ViewBag.TotalOrders = data.DonHangs.Count();

            // Đơn hàng mới nhất
            ViewBag.RecentOrders = data.DonHangs
                .OrderByDescending(d => d.NgayDatHang)
                .Take(5)
                .ToList();

            // Sản phẩm mới nhất
            ViewBag.RecentProducts = data.SanPhams
                .OrderByDescending(s => s.IdProduct)
                .Take(5)
                .ToList();

            // Thống kê đơn hàng theo trạng thái
            ViewBag.PendingOrders = data.DonHangs.Count(d => d.TrangThai == "Chờ xử lý");
            ViewBag.ProcessingOrders = data.DonHangs.Count(d => d.TrangThai == "Đã duyệt");
            ViewBag.ShippingOrders = data.DonHangs.Count(d => d.TrangThai == "Đang giao");
            ViewBag.CompletedOrders = data.DonHangs.Count(d => d.TrangThai == "Đã giao");
            ViewBag.CancelledOrders = data.DonHangs.Count(d => d.TrangThai == "Đã hủy");

            return View();
        }

        // Xem danh sách đơn hàng
        public ActionResult DonHang()
        {
            var danhSach = data.DonHangs.OrderByDescending(d => d.NgayDatHang).ToList();
            return View(danhSach);
        }

        // Xem chi tiết đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            var chiTiet = data.ChiTietDonHangs.Where(c => c.IdDonHang == id).ToList();
            ViewBag.DonHang = data.DonHangs.FirstOrDefault(d => d.IdDonHang == id);
            return View(chiTiet);
        }

        // Cập nhật trạng thái đơn hàng
        public ActionResult CapNhatTrangThai(int id, string trangThai)
        {
            var don = data.DonHangs.FirstOrDefault(d => d.IdDonHang == id);
            if (don != null)
            {
                don.TrangThai = trangThai;
                data.SubmitChanges();
            }
            return RedirectToAction("DonHang");
        }

        #region Quản lý sản phẩm
        public ActionResult SanPham()
        {
            var danhSach = data.SanPhams.OrderByDescending(s => s.IdProduct).ToList();
            return View(danhSach);
        }

        public ActionResult ThemSanPham()
        {
            ViewBag.DanhMucs = data.DanhMucGiays.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, HttpPostedFileBase HinhAnh)
        {
            if (HinhAnh != null)
            {
                string path = Server.MapPath("~/Content/Images/");
                string filename = HinhAnh.FileName;
                HinhAnh.SaveAs(path + filename);
                sp.HinhAnh = filename;
            }
            data.SanPhams.InsertOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }

        public ActionResult SuaSanPham(int id)
        {
            var sp = data.SanPhams.FirstOrDefault(s => s.IdProduct == id);
            ViewBag.DanhMucs = data.DanhMucGiays.ToList();
            return View(sp);
        }

        [HttpPost]
        public ActionResult SuaSanPham(SanPham sp, HttpPostedFileBase HinhAnh)
        {
            var sanPham = data.SanPhams.FirstOrDefault(s => s.IdProduct == sp.IdProduct);
            if (sanPham != null)
            {
                if (HinhAnh != null)
                {
                    string path = Server.MapPath("~/Content/Images/");
                    string filename = HinhAnh.FileName;
                    HinhAnh.SaveAs(path + filename);
                    sanPham.HinhAnh = filename;
                }
                sanPham.TenSanPham = sp.TenSanPham;
                sanPham.MoTa = sp.MoTa;
                sanPham.Gia = sp.Gia;
                sanPham.IdDanhMuc = sp.IdDanhMuc;
                data.SubmitChanges();
            }
            return RedirectToAction("SanPham");
        }

        public ActionResult XoaSanPham(int id)
        {
            var sp = data.SanPhams.FirstOrDefault(s => s.IdProduct == id);
            if (sp != null)
            {
                data.SanPhams.DeleteOnSubmit(sp);
                data.SubmitChanges();
            }
            return RedirectToAction("SanPham");
        }
        #endregion

        #region Quản lý người dùng
        public ActionResult KhachHang()
        {
            var danhSach = data.KhachHangs.OrderByDescending(k => k.IdUser).ToList();
            return View(danhSach);
        }

        public ActionResult SuaKhachHang(int id)
        {
            var kh = data.KhachHangs.FirstOrDefault(k => k.IdUser == id);
            return View(kh);
        }

        [HttpPost]
        public ActionResult SuaKhachHang(KhachHang kh)
        {
            var khachHang = data.KhachHangs.FirstOrDefault(k => k.IdUser == kh.IdUser);
            if (khachHang != null)
            {
                khachHang.HoTen = kh.HoTen;
                khachHang.Email = kh.Email;
                khachHang.DiaChi = kh.DiaChi;
                khachHang.DienThoai = kh.DienThoai;
                khachHang.UserRole = kh.UserRole;
                data.SubmitChanges();
            }
            return RedirectToAction("KhachHang");
        }

        public ActionResult XoaKhachHang(int id)
        {
            var kh = data.KhachHangs.FirstOrDefault(k => k.IdUser == id);
            if (kh != null)
            {
                data.KhachHangs.DeleteOnSubmit(kh);
                data.SubmitChanges();
            }
            return RedirectToAction("KhachHang");
        }
        #endregion

        #region Quản lý danh mục
        public ActionResult DanhMuc()
        {
            var danhSach = data.DanhMucGiays.OrderByDescending(d => d.IdDanhMuc).ToList();
            return View(danhSach);
        }

        public ActionResult ThemDanhMuc()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemDanhMuc(DanhMucGiay dm)
        {
            try
            {
                data.DanhMucGiays.InsertOnSubmit(dm);
                data.SubmitChanges();
                return RedirectToAction("DanhMuc");
            }
            catch (Exception)
            {
                return View(dm);
            }
        }

        public ActionResult SuaDanhMuc(int id)
        {
            var dm = data.DanhMucGiays.FirstOrDefault(d => d.IdDanhMuc == id);
            if (dm == null)
            {
                return RedirectToAction("DanhMuc");
            }
            return View(dm);
        }

        [HttpPost]
        public ActionResult SuaDanhMuc(DanhMucGiay dm)
        {
            try
            {
                var danhMuc = data.DanhMucGiays.FirstOrDefault(d => d.IdDanhMuc == dm.IdDanhMuc);
                if (danhMuc != null)
                {
                    danhMuc.TenDanhMuc = dm.TenDanhMuc;
                    data.SubmitChanges();
                }
                return RedirectToAction("DanhMuc");
            }
            catch (Exception)
            {
                return View(dm);
            }
        }

        public ActionResult XoaDanhMuc(int id)
        {
            try
            {
                var dm = data.DanhMucGiays.FirstOrDefault(d => d.IdDanhMuc == id);
                if (dm != null)
                {
                    // Kiểm tra xem danh mục có sản phẩm không
                    var hasSanPham = data.SanPhams.Any(s => s.IdDanhMuc == id);
                    if (!hasSanPham)
                    {
                        data.DanhMucGiays.DeleteOnSubmit(dm);
                        data.SubmitChanges();
                        TempData["ThongBao"] = "Xóa danh mục thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Không thể xóa danh mục này vì đang có sản phẩm thuộc danh mục!";
                    }
                }
                return RedirectToAction("DanhMuc");
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa danh mục!";
                return RedirectToAction("DanhMuc");
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    public class GioHangController : BaseController
    {
        //
        // GET: /GioHang/

        public ActionResult Index()
        {
            // Kiểm tra đăng nhập
            var user = Session["User"] as KhachHang;
            if (user == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var gioHang = Session["GioHang"] as List<CartItem> ?? new List<CartItem>();
            
            // Cập nhật thông tin sản phẩm từ database
            foreach (var item in gioHang)
            {
                var sp = db.SanPhams.FirstOrDefault(s => s.IdProduct == item.ProductId);
                if (sp != null)
                {
                    item.ProductName = sp.TenSanPham;
                    item.Image = sp.HinhAnh;
                    item.Price = sp.Gia;
                }
            }
            
            return View(gioHang);
        }

        public ActionResult ThemVaoGio(int maSP, string size = "39", int soLuong = 1, string returnUrl = null)
        {
            var user = Session["User"] as KhachHang;
            if (user == null || user.UserRole != 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var sp = db.SanPhams.FirstOrDefault(s => s.IdProduct == maSP);
            if (sp == null) return HttpNotFound();

            var gioHang = Session["GioHang"] as List<CartItem> ?? new List<CartItem>();

            var existing = gioHang.FirstOrDefault(g => g.ProductId == maSP && g.Size == size);
            if (existing != null)
            {
                existing.Quantity += soLuong; 
            }
            else
            {
                gioHang.Add(new CartItem
                {
                    ProductId = sp.IdProduct,
                    ProductName = sp.TenSanPham,
                    Image = sp.HinhAnh,
                    Price = sp.Gia,
                    Quantity = soLuong, 
                    Size = size
                });
            }

            Session["GioHang"] = gioHang;

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "SanPham");
        }


        public ActionResult XoaKhoiGio(int maSP)
        {
            var gioHang = Session["GioHang"] as List<CartItem>;
            if (gioHang != null)
            {
                var item = gioHang.FirstOrDefault(g => g.ProductId == maSP);
                if (item != null)
                {
                    gioHang.Remove(item);
                    Session["GioHang"] = gioHang;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CapNhatSoLuong(int maSP, int soLuong)
        {
            if (soLuong < 1) soLuong = 1;

            var gioHang = Session["GioHang"] as List<CartItem>;
            if (gioHang != null)
            {
                var item = gioHang.FirstOrDefault(g => g.ProductId == maSP);
                if (item != null)
                {
                    item.Quantity = soLuong;
                    Session["GioHang"] = gioHang;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CapNhatSize(int maSP, string size)
        {
            if (string.IsNullOrEmpty(size)) size = "39";

            var gioHang = Session["GioHang"] as List<CartItem>;
            if (gioHang != null)
            {
                var item = gioHang.FirstOrDefault(g => g.ProductId == maSP);
                if (item != null)
                {
                    // Kiểm tra xem đã có sản phẩm cùng loại khác size chưa
                    var existingWithSize = gioHang.FirstOrDefault(g => g.ProductId == maSP && g.Size == size && g != item);
                    if (existingWithSize != null)
                    {
                        // Nếu có, cộng số lượng vào và xóa item cũ
                        existingWithSize.Quantity += item.Quantity;
                        gioHang.Remove(item);
                    }
                    else
                    {
                        // Nếu không, cập nhật size mới
                        item.Size = size;
                    }
                    Session["GioHang"] = gioHang;
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult XoaGioHang()
        {
            Session.Remove("GioHang");
            return RedirectToAction("Index");
        }

        private double TinhTongTien(List<CartItem> gioHang)
        {
            if (gioHang == null)
                return 0;

            return gioHang.Sum(item => item.Price * item.Quantity);
        }
    }
}

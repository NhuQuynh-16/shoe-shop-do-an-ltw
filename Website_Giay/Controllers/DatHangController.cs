using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    public class DatHangController : BaseController
    {
        //
        // GET: /DatHang/


        // GET: Hiển thị form nhập địa chỉ
        public ActionResult Checkout()
        {
            if (Session["User"] == null)
            {
                TempData["ReturnUrl"] = "/DatHang/Checkout";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var cart = Session["GioHang"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "GioHang");

            return View(cart);
        }

        // POST: Xử lý lưu đơn hàng
        [HttpPost]
        public ActionResult Checkout(string diaChi)
        {
            try
            {
                var cart = Session["GioHang"] as List<CartItem>;
                if (cart == null || !cart.Any())
                    return RedirectToAction("Index", "GioHang");

                int userId = (int)Session["UserId"];
                var user = db.KhachHangs.FirstOrDefault(u => u.IdUser == userId);
                if (user == null)
                    return RedirectToAction("DangNhap", "TaiKhoan");

                var donHang = new DonHang
                {
                    IdUser = userId,
                    NgayDatHang = DateTime.Now,
                    TongTien = cart.Sum(c => c.Price * c.Quantity),
                    DiaChiGiaoHang = diaChi,
                    TrangThai = "Chờ xử lý"
                };

                db.DonHangs.InsertOnSubmit(donHang);
                db.SubmitChanges();

                foreach (var item in cart)
                {
                    var ct = new ChiTietDonHang
                    {
                        IdDonHang = donHang.IdDonHang,
                        IdProduct = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.Price,
                        Size = item.Size ?? "39" // Sử dụng size mặc định là 39 nếu không có size được chọn
                    };
                    db.ChiTietDonHangs.InsertOnSubmit(ct);
                }

                db.SubmitChanges();
                Session.Remove("GioHang");

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                // Log lỗi và hiển thị thông báo
                TempData["Error"] = "Có lỗi xảy ra khi đặt hàng: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }

        public ActionResult Success()
        {
            return View();
        }

    }
}

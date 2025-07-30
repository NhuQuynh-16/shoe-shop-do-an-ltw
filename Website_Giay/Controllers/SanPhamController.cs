using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    public class SanPhamController : BaseController
    {
        //
        // GET: /SanPham/
        public ActionResult Index(string search, int? danhmuc)
        {
            // Lấy vai trò người dùng từ session
            ViewBag.UserRole = Session["UserRole"];
            
            // Lấy danh sách danh mục để hiển thị trong sidebar
            ViewBag.DanhMucs = db.DanhMucGiays.ToList();

            // Lấy tất cả sản phẩm
            List<SanPham> products = db.SanPhams.ToList();
            
            // Lọc theo danh mục nếu có
            if (danhmuc.HasValue)
            {
                List<SanPham> filteredProducts = new List<SanPham>();
                foreach (var product in products)
                {
                    if (product.IdDanhMuc == danhmuc.Value)
                    {
                        filteredProducts.Add(product);
                    }
                }
                products = filteredProducts;
                ViewBag.SelectedCategory = danhmuc.Value;
            }
            
            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                List<SanPham> searchResults = new List<SanPham>();
                foreach (var product in products)
                {
                    if (product.TenSanPham.ToLower().Contains(search.ToLower()))
                    {
                        searchResults.Add(product);
                    }
                }
                products = searchResults;
            }

            return View(products);
        }

        public ActionResult ChiTiet(int id)
        {
            ViewBag.DanhMucs = db.DanhMucGiays.ToList();
            var sp = db.SanPhams.FirstOrDefault(s => s.IdProduct == id);
            if (sp == null) return HttpNotFound();

            //san pham goi y
            //method chaining nhung xuong dong :)))
            var suggest = db.SanPhams
             .Where(s => s.IdDanhMuc == sp.IdDanhMuc && s.IdProduct != id)
             .Take(4)
             .ToList();

            ViewBag.suggest_product = suggest;
            return View(sp);
        }
    }
}

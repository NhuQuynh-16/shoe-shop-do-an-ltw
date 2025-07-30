using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;

namespace Website_Giay.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index(string search)
        {

            // Lấy vai trò người dùng từ session
            ViewBag.UserRole = Session["UserRole"];

            var products = from sp in db.SanPhams
                           where string.IsNullOrEmpty(search) || sp.TenSanPham.Contains(search)
                           select sp;

            return View(products.ToList());
        }

    }
}

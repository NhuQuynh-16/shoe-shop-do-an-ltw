using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Giay.Models;
using System.Data.SqlClient;

namespace Website_Giay.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/


        protected DataClasses1DataContext db = new DataClasses1DataContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ViewBag.DanhMucs = db.DanhMucGiays.ToList();
            }
            catch
            {
                ViewBag.DanhMucs = new List<DanhMucGiay>();
            }

            base.OnActionExecuting(filterContext);
        }

    }
}

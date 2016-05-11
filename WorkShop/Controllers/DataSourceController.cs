using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkShop.Models;

namespace WorkShop.Controllers
{
    public class DataSourceController : Controller
    {
        // GET: DataSource
        [HttpGet]
        public JsonResult Data()
        {

            WorkShopDB db = new WorkShopDB();

            Orders data = db.Orders.Find(10248);

            String id = data.OrderID.ToString();
            String name = data.Customers.CompanyName;
            String orderDate = data.OrderDate.ToString();
            String shipDate = data.ShippedDate.ToString();

            var obj = new
            {
                id = id,
                name = name,
                orderDate = orderDate,
                shipDate = shipDate
            };

            return Json(obj);
        }
    }
}
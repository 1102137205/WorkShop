using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkShop.Models;

namespace WorkShop.Controllers
{
    public class SearchDataController : Controller
    {
        WorkShopDB db = new WorkShopDB();

        // GET: SearchData
        public ActionResult Index()
        {

            List<String> employeeName = db.Employees.Select(x=>x.LastName).ToList();
            List<String> companyName = db.Shippers.Select(x=>x.CompanyName).ToList();

            ViewBag.emName = employeeName;
            ViewBag.coName = companyName;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection inputs)
        {
            String orderId = inputs["orderId"];
            String custName = inputs["custName"];
            String employee = inputs["employee"];
            String ship = inputs["ship"];
            String orderDate = inputs["orderDate"];
            String shipDate = inputs["shipDate"];
            String needDate = inputs["needDate"];

            return RedirectToAction("search",
                new { orderId, custName, employee, ship , orderDate, shipDate, needDate});
        }

        public ActionResult search(String orderId, String custName,String employee,
            String ship, String orderDate, String shipDate, String needDate)
        {

            DateTime orderD = Convert.ToDateTime(orderDate);
            DateTime shipD = Convert.ToDateTime(shipDate);
            DateTime needD = Convert.ToDateTime(needDate);

            List<Orders> orderdata = db.Orders.Where(x =>
                (String.IsNullOrEmpty(orderId) ? true : x.OrderID.ToString() == orderId) &&
                (String.IsNullOrEmpty(custName) ? true : x.Customers.CompanyName.Contains(custName)) &&
                (employee.Equals("00") ? true : x.Employees.LastName.Equals(employee)) &&
                (ship.Equals("00") ? true : x.Shippers.CompanyName.Equals(ship)) &&
                (String.IsNullOrEmpty(orderDate) ? true : x.OrderDate == orderD) &&
                (String.IsNullOrEmpty(shipDate) ? true : x.ShippedDate == shipD) &&
                (String.IsNullOrEmpty(needDate) ? true : x.RequiredDate == needD)

                ).ToList();

            ViewBag.orderdata = orderdata;

            return View();
        }

    }
}
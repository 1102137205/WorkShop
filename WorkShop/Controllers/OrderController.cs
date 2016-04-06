using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WorkShop.Models;

namespace WorkShop.Controllers
{
    public class OrderController : Controller
    {
        WorkShopDB db = new WorkShopDB();

        // GET: Order
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

        [HttpPost]
        public ActionResult search(FormCollection inputs)
        {
            String orderId = inputs["OrderId"];
            return RedirectToAction("update", new { orderId });
        }

        public ActionResult update(String orderId)
        {

            #region 填入 table 資料
            List<Orders> OrderData = db.Orders.Where(x => x.OrderID.ToString() == orderId).ToList();
            Orders data = new Orders();

            String CompanyName = "";
            String LastName = "";

            String OrderDate = "";
            String orderMonth = "";
            String orderDay = "";
            //String RequiredDate = "";
            //String ShippedDate = "";


            foreach (var tmp in OrderData) {
                data.OrderID = tmp.OrderID;
                CompanyName = tmp.Customers.CompanyName;
                LastName = tmp.Employees.LastName;

                orderMonth = DateFormat(tmp.OrderDate.Month.ToString());
                orderDay = DateFormat(tmp.OrderDate.Day.ToString());

                OrderDate = String.Format("{0}-{1}-{2}", tmp.OrderDate.Year, orderMonth, orderDay);

                //date.GetDateTimeFormats('s')[0].ToString();
                //data.RequiredDate = tmp.RequiredDate;
                //data.ShippedDate = tmp.ShippedDate;

                data.ShipName = tmp.ShipName;
                data.Freight = tmp.Freight;
                data.ShipCountry = tmp.ShipCountry;
                data.ShipCity = tmp.ShipCity;
                data.ShipRegion = tmp.ShipRegion;
                data.ShipPostalCode = tmp.ShipPostalCode;
                data.ShipAddress = tmp.ShipAddress;
            }



            ViewBag.OrderData = data;
            ViewBag.LastName = LastName;
            ViewBag.CompanyName = CompanyName;
            ViewBag.OrderDate = OrderDate;

            List<String> CompanyNameList = db.Customers.Where(x => x.CompanyName != CompanyName).Select(x => x.CompanyName).ToList();
            ViewBag.CompanyNameList = CompanyNameList;

            List<String> lastNameList = db.Employees.Where(x => x.LastName != LastName).Select(x => x.LastName).ToList();
            ViewBag.lastNameList = lastNameList;

            List<String> shipNameList = db.Orders.Where(x=>x.ShipName != data.ShipName  ).Select(x => x.ShipName).ToList();
            ViewBag.shipNameList = shipNameList;

            #endregion

            #region 計算總金額

            Double total = 0; 

            List<OrderDetails> OrderDetail = db.OrderDetails.Where(x => x.OrderID.ToString() == orderId).ToList();

            foreach (var tmp in OrderDetail)
            {
                total = total + tmp.Qty * Convert.ToDouble(tmp.UnitPrice) - Convert.ToDouble(tmp.Discount);
            }

            total = total + Convert.ToDouble(data.Freight);
            ViewBag.total = total;

            #endregion

            return View();
        }

        public String DateFormat(String date) {

            StringBuilder sb = new StringBuilder();

            if (date.Length < 2)
            {
                sb.Append("0");
                sb.Append(date);
            }

            return sb.ToString();
        }

    }
}
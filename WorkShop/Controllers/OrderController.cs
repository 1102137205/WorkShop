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


        public ActionResult update(String orderId)
        {
            
            #region 填入 table 資料
            List<Orders> OrderData = db.Orders.Where(x => x.OrderID.ToString() == orderId).ToList();

            Orders data = new Orders();

            String CustomersCompanyName = "";
            String CustomersCompanyId = "";
            String LastName = "";
            String LastId = "";
            String ShippersCompanyName = "";
            String ShippersCompanyId = "";

            String OrderDate = "";
            String orderMonth = "";
            String orderDay = "";

            String RequiredDate = "";
            String requiredMonth = "";
            String requiredDay = "";

            String ShippedDate = "";
            String shippedMonth = "";
            String shippedDay = "";

            foreach (var tmp in OrderData) {
                data.OrderID = tmp.OrderID;
                CustomersCompanyName = tmp.Customers.CompanyName;
                CustomersCompanyId = tmp.CustomerID.ToString();
                LastName = tmp.Employees.LastName;
                LastId = tmp.EmployeeID.ToString() ;
                
                orderMonth = DateFormat(tmp.OrderDate.Month.ToString());
                orderDay = DateFormat(tmp.OrderDate.Day.ToString());
                OrderDate = String.Format("{0}-{1}-{2}", tmp.OrderDate.Year, orderMonth, orderDay);

                requiredMonth = DateFormat(tmp.RequiredDate.Month.ToString());
                requiredDay = DateFormat(tmp.RequiredDate.Day.ToString());
                RequiredDate = String.Format("{0}-{1}-{2}", tmp.RequiredDate.Year, requiredMonth, requiredDay);

                shippedMonth = DateFormat(Convert.ToDateTime(tmp.ShippedDate).Month.ToString());
                shippedDay = DateFormat(Convert.ToDateTime(tmp.ShippedDate).Day.ToString());
                ShippedDate = String.Format("{0}-{1}-{2}", Convert.ToDateTime(tmp.ShippedDate).Year, shippedMonth, shippedDay);

                ShippersCompanyName = tmp.Shippers.CompanyName;
                ShippersCompanyId = tmp.ShipperID.ToString();
                data.Freight = tmp.Freight;
                data.ShipCountry = tmp.ShipCountry;
                data.ShipCity = tmp.ShipCity;
                data.ShipRegion = tmp.ShipRegion;
                data.ShipPostalCode = tmp.ShipPostalCode;
                data.ShipAddress = tmp.ShipAddress;
                data.ShipName = tmp.ShipName;
            }

            ViewBag.OrderData = data;

            ViewBag.LastName = LastName;
            ViewBag.LastId = LastId;

            ViewBag.CustomersCompanyName = CustomersCompanyName;
            ViewBag.CustomersCompanyId = CustomersCompanyId;

            ViewBag.ShippersCompanyName = ShippersCompanyName;
            ViewBag.ShippersCompanyId = ShippersCompanyId;

            ViewBag.OrderDate = OrderDate;
            ViewBag.RequiredDate = RequiredDate;
            ViewBag.ShippedDate = ShippedDate;

            List<Customers> CompanyList = db.Customers.Where(x => x.CompanyName != CustomersCompanyName).ToList();
            ViewBag.CompanyList = CompanyList;

            List<Employees> lastNameList = db.Employees.Where(x => x.LastName != LastName).ToList();
            ViewBag.lastNameList = lastNameList;

            List<Shippers> shipList = db.Shippers.Where(x => x.CompanyName != ShippersCompanyName).ToList();
            ViewBag.shipList = shipList;

            

            #endregion

            #region 填入明細資料
            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID.ToString() == orderId).ToList();

            List<Products> ProductData = db.Products.ToList();
            ViewBag.orderDetail = orderDetail;

            ViewBag.ProductData = ProductData;

            #endregion

            #region 計算總金額

            Double total = 0;

            foreach (var tmp in orderDetail)
            {
                total = total + tmp.Qty * Convert.ToDouble(tmp.UnitPrice);
            }

            total = Math.Round(total + Convert.ToDouble(data.Freight));
            ViewBag.total = total;

            #endregion

            return View();
        }

        public String OrderPrice(int productId) {

            String changPrice = db.Products.Find(productId).UnitPrice.ToString();

            return changPrice;
        }



        public String DateFormat(String date)
        {

            StringBuilder sb = new StringBuilder();

            if (date.Length < 2)
            {
                sb.Append("0");
            }
            sb.Append(date);

            return sb.ToString();
        }

        [HttpPost]
        public ActionResult update(FormCollection inputs)
        {
            #region 修改table資料
            String orderId = inputs["orderId"];
            String custId = inputs["custName"];
            String employeeId = inputs["employeeName"];
            String orderDate = inputs["orderDate"];
            String needDate = inputs["needDate"];
            String shipDate = inputs["shipDate"];
            String shipCompanyId = inputs["shipCompanyName"];
            String shipCost = inputs["shipCost"];
            String shipCountry = inputs["shipCountry"];
            String shipCity = inputs["shipCity"];
            String shipRegion = inputs["shipRegion"];
            String shipAddressNo = inputs["shipAddressNo"];
            String shipAddress = inputs["shipAddress"];
            String shipDesc = inputs["shipDesc"];

            Orders data = db.Orders.Find(Convert.ToInt32(orderId));
            
            data.CustomerID = Convert.ToInt32(custId);
            data.EmployeeID = Convert.ToInt32(employeeId);
            data.OrderDate = Convert.ToDateTime(orderDate);
            data.RequiredDate = Convert.ToDateTime(needDate);
            data.ShippedDate = Convert.ToDateTime(shipDate);
            data.ShipperID = Convert.ToInt32(shipCompanyId);
            data.Freight = Convert.ToDecimal(shipCost);
            data.ShipCountry = shipCountry;
            data.ShipCity = shipCity;
            data.ShipRegion = shipRegion;
            data.ShipPostalCode = shipAddressNo;
            data.ShipAddress = shipAddress;
            data.ShipName = shipDesc;
            #endregion

            #region 修改明細資料

            int count = 0;

            for (int i = 1; i < inputs.Count; i++)
            {
                if (inputs.AllKeys.Contains("productName[" + i + "]"))
                {
                    count++;
                }
            }

            List<OrderDetails> dataDetail = db.OrderDetails.Where(x=>x.OrderID ==data.OrderID).ToList();

            int j = 0;
            foreach (var tmp in dataDetail)
	        {
                j++;
                tmp.ProductID = Convert.ToInt32(inputs["productName[" + j + "]"]);
                tmp.UnitPrice = Convert.ToDecimal(inputs["price[" + j + "]"]);
                tmp.Qty = Convert.ToInt16(inputs["qty[" + j + "]"]);

                data.OrderDetails.Add(tmp);
	        }

            for (int i = j+1; i <= count; i++)
            {
                OrderDetails addOrderDetail = new OrderDetails();

                addOrderDetail.OrderID = data.OrderID;
                addOrderDetail.ProductID = Convert.ToInt32(inputs["productName[" + i + "]"]);
                addOrderDetail.UnitPrice = Convert.ToDecimal(inputs["price[" + i + "]"]);
                addOrderDetail.Qty = Convert.ToInt16(inputs["qty[" + i + "]"]);

                data.OrderDetails.Add(addOrderDetail);
            }

            #endregion

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Insert() {

            List<Customers> customer = db.Customers.ToList();
            List<Employees> employee = db.Employees.ToList();
            List<Shippers> ShipCompany = db.Shippers.ToList();
            List<Products> ProductData = db.Products.ToList();

            int orderId = db.Orders.Select(x=>x.OrderID).Max()+1;

            ViewBag.customer = customer;
            ViewBag.employee = employee;
            ViewBag.ShipCompany = ShipCompany;
            ViewBag.orderId = orderId;
            ViewBag.ProductData = ProductData;
            return View();
        }

        [HttpPost]
        public ActionResult Insert(FormCollection inputs) {

            String orderId = inputs["orderId"];
            String custId= inputs["custName"];
            String employeeId = inputs["employeeName"];
            String orderDate = inputs["orderDate"];
            String needDate = inputs["needDate"];
            String shipDate = inputs["shipDate"];
            String shipCompanyId = inputs["shipCompanyName"];
            String shipCost = inputs["shipCost"];
            String shipCountry = inputs["shipCountry"];
            String shipCity = inputs["shipCity"];
            String shipRegion = inputs["shipRegion"];
            String shipAddressNo = inputs["shipAddressNo"];
            String shipAddress = inputs["shipAddress"];
            String shipDesc = inputs["shipDesc"];

            Orders data = new Orders();
            
            data.OrderID = Convert.ToInt32(orderId);
            data.CustomerID= Convert.ToInt32(custId);
            data.EmployeeID = Convert.ToInt32(employeeId);
            data.OrderDate = Convert.ToDateTime(orderDate);
            data.RequiredDate = Convert.ToDateTime(needDate);
            data.ShippedDate = !String.IsNullOrEmpty(shipDate) ? Convert.ToDateTime(shipDate) : Convert.ToDateTime("");
            data.ShipperID = Convert.ToInt32(shipCompanyId);
            data.Freight = !String.IsNullOrEmpty(shipCost) ? Convert.ToDecimal(shipCost) : 0;
            data.ShipCountry = !String.IsNullOrEmpty(shipCost) ? shipCountry :"";
            data.ShipCity = !String.IsNullOrEmpty(shipCost) ? shipCity : "";
            data.ShipRegion = !String.IsNullOrEmpty(shipRegion) ? shipRegion : "";
            data.ShipPostalCode = !String.IsNullOrEmpty(shipAddressNo) ? shipAddressNo : "";
            data.ShipAddress = !String.IsNullOrEmpty(shipAddress) ? shipAddress : "";
            data.ShipName = !String.IsNullOrEmpty(shipDesc) ? shipDesc : "";

            db.Orders.Add(data);

            int count = 0;

            for (int i = 1; i < inputs.Count; i++)
            {
                if (inputs.AllKeys.Contains("productName[" + i + "]"))
                {
                    count++;
                }
            }

            for (int i = 1; i <= count; i++)
            {
                OrderDetails dataDetail = new OrderDetails();

                dataDetail.OrderID = data.OrderID;
                dataDetail.ProductID = Convert.ToInt32(inputs["productName[" + i + "]"]);
                dataDetail.UnitPrice = Convert.ToDecimal(inputs["price[" + i + "]"]);
                dataDetail.Qty = Convert.ToInt16(inputs["qty[" + i + "]"]);
                dataDetail.Orders = data;
                data.OrderDetails.Add(dataDetail);

            }


            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public void delete(int orderId) {
            Orders order = db.Orders.Find(orderId);
            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID == orderId).ToList();

            db.Orders.Remove(order);
            db.OrderDetails.RemoveRange(orderDetail);
            db.SaveChanges();
        }

    }
}
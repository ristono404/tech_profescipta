using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using profescipta.Models;
using profescipta.Repository;
using System.Linq.Dynamic.Core;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace profescipta.Controllers;

public class SalesOrderController : Controller
{
     private IUnitOfWork _unitOfWork { get; set; }

    public SalesOrderController(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }


public IActionResult Index()
{
     ViewData["Title"] = "SALES ORDER";
     var result = _unitOfWork.Order.GetAllQueryable().ToList();
     return View(result);
}

public IActionResult LoadData()
{
    try
    {
        var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
        // Skip number of Rows count
        var start = Request.Form["start"].FirstOrDefault();
        // Paging Length 10, 20
        var length = Request.Form["length"].FirstOrDefault();
        // Sort Column Name
        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        // Sort Column Direction (asc, desc)
        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        // Search Value from (Search box)
       // var searchValue = Request.Form["search[value]"].FirstOrDefault();
        var keywords = Request.Form["keywords"].FirstOrDefault();
        var orderDate = Request.Form["order_date"].FirstOrDefault();//09/08/2024
        // Paging Size (10, 20, 50, 100)
        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        int skip = start != null ? Convert.ToInt32(start) : 0;
        int recordsTotal = 0;
       var result = _unitOfWork.Order.GetAllQueryable();

        if (!string.IsNullOrEmpty(keywords) && !string.IsNullOrEmpty(orderDate) )
        {
            DateTime parsedOrderDate;
            if (DateTime.TryParseExact(orderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedOrderDate))
            {
                result = result.Where(m => (m.order_date.Date == parsedOrderDate.Date) && (m.Customer.customer_name.Contains(keywords) || m.order_no.Contains(keywords)));
            }
        }else if (!string.IsNullOrEmpty(keywords) )
        {
            result = result.Where(m => m.Customer.customer_name.Contains(keywords) || m.order_no.Contains(keywords));
        }else if (!string.IsNullOrEmpty(orderDate))
        {
            DateTime parsedOrderDate;
            if (DateTime.TryParseExact(orderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedOrderDate))
            {
                result = result.Where(m => m.order_date.Date == parsedOrderDate.Date);
            }
        }
        // total number of rows counts
        recordsTotal = result.Count();
        // Paging
        var data = result .Select(order => new GridSalesOrderViewModel
        {
            so_order_id = order.so_order_id,
            order_no = order.order_no,
            order_date = order.order_date.ToString("dd/MM/yyyy"),
            customer = order.Customer.customer_name 
        }).Skip(skip).Take(pageSize).ToList();

        // Returning Json Data
        return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
    }
    catch (Exception)
    {
        throw;
    }
}

public IActionResult Add()
{
     ViewData["Title"] = "ADD NEW - SALES ORDER";
     var result = _unitOfWork.Customer.GetAllQueryable().ToList();
     return View(result);
}

[HttpPost]
public JsonResult Add([FromBody] CreateOrderViewModel data)
{
    ResultModel result = new ResultModel();
    if (data == null)
    {
        return Json(new { success = false, message = "Data is null" });
    }

   var totalData = _unitOfWork.Order.GetAllQueryable().Where(x => x.order_no == data.OrderNo).Count();
   if (totalData > 0)
    {
        result.httpStatus = 500;
        result.Message = "The order number is already in use. Please choose a different one.";
        return Json(result);
    }

        // Create Order entity
    Order orderEntity = new Order()
    {
        order_no = data.OrderNo,
        order_date = data.OrderDate,
        com_customer_id = data.ComCustomerId,
        address = data.Address,
       Items = new List<Item>()  // Initialize the list to add items later
    };

    // Loop through the items in the view model and create Item entities
    foreach (var item in data.Items)
    {
        var itemEntity = new Item()
        {
            item_name = item.item_name,
            quantity = item.quantity,
            price = item.price
            // so_order_id will be automatically set after orderEntity is saved
        };

        // Add each item to the order's Items collection
        orderEntity.Items.Add(itemEntity);
    }

    // Add the order (along with its items) to the database
    _unitOfWork.Order.Add(orderEntity);
    _unitOfWork.Save();
             
            result.httpStatus = 200;
            result.Message = "Success Save Data";

    // Your processing logic here
    return Json(result);  // For now, return the received data
}

public IActionResult Edit(int orderId)
{
    ViewData["Title"] = "EDIT - SALES ORDER";
    
    var order = _unitOfWork.Order.GetAllQueryable().Select(x => new SalesOrderViewModel{
            so_order_id = x.so_order_id,
            order_no = x.order_no,
            order_date = x.order_date.ToString("dd/MM/yyyy"),
            com_customer_id = x.com_customer_id ,
            address = x.address
    }).Where(x => x.so_order_id == orderId).FirstOrDefault();
    if (order == null)
    {
        return RedirectToAction("Index");
    }
    
    // You can add more data to ViewData
    var customerData = _unitOfWork.Customer.GetAllQueryable().Select( cust =>
    new SelectListItem
    {
        Value = cust.com_customer_id.ToString(), 
        Text = cust.customer_name ,
    }).ToList();

     ViewBag.Customers = new SelectList(customerData, "Value", "Text", order.com_customer_id); //
    ViewBag.SelectedCustomerId = order.com_customer_id;
    ViewBag.Order = order;
    // ViewBag.Customers = customerData;

    return View();
}

[HttpPost]
public JsonResult Edit([FromBody] CreateOrderViewModel data)
{
    ResultModel result = new ResultModel();


   var totalData = _unitOfWork.Order.GetAllQueryable().Where(x => x.order_no == data.OrderNo && x.so_order_id != data.OrderId).Count();
   if (totalData > 0)
    {
        result.httpStatus = 500;
        result.Message = "The order number is already in use. Please choose a different one.";
        return Json(result);
    }

    var order = _unitOfWork.Order.GetAllQueryable().Include("Items").Where(x => x.so_order_id == data.OrderId).FirstOrDefault();
    if (order == null)
    {
       result.httpStatus = 500;
        result.Message = "Data Not Found";
        return Json(result);
    }

    order.order_no = data.OrderNo;
    order.com_customer_id = data.ComCustomerId;
    order.order_date= data.OrderDate;
    order.address = data.Address;

     var existingItems = order.Items.ToList(); 
     _unitOfWork.Item.DeleteRange(existingItems);
     
    // Loop through the items in the view model and create Item entities
    foreach (var item in data.Items)
    {
        var itemEntity = new Item()
        {
            item_name = item.item_name,
            quantity = item.quantity,
            price = item.price
            // so_order_id will be automatically set after orderEntity is saved
        };

        // Add each item to the order's Items collection
        order.Items.Add(itemEntity);
    }

    // Add the order (along with its items) to the database
    _unitOfWork.Order.Update(order);
    _unitOfWork.Save();
             
    result.httpStatus = 200;
    result.Message = "Success Save Data";

    // Your processing logic here
    return Json(result);  // For now, return the received data
}


public IActionResult LoadItemData(string orderNo)
{
    try
    {
       var order = _unitOfWork.Order.GetAllQueryable().Include("Items").Where(x => x.order_no == orderNo).FirstOrDefault();
       if (order == null)
       {
            return RedirectToAction("Index");
       }
       
        // Paging
        var data = order.Items.Select(item => new ItemViewModel
        {
            so_item_id = item.so_item_id,
            item_name = item.item_name,
            quantity = item.quantity,
            price = item.price,
            total = item.quantity * item.price
        }).ToList();

        // Returning Json Data
        return Json(data);
    }
    catch (Exception)
    {
        throw;
    }
}


[HttpGet]
public IActionResult Delete(string orderNo)
{
    ResultModel result = new ResultModel();
    var checkdata = _unitOfWork.Order.GetAllQueryable().Where(x => x.order_no == orderNo).FirstOrDefault();
    if (checkdata == null)
    {
        result.httpStatus = 500;
        result.Message = "Data Not Found";
        return Ok(result);
    
    }
    _unitOfWork.Order.Delete(checkdata);
    _unitOfWork.Save();
    result.httpStatus = 200;
    result.Message = "Success Delete Data";
    return Ok(result);
}

public IActionResult ExportToExcel(string keywords, string orderDate)
    {
        try
        {
            var result = _unitOfWork.Order.GetAllQueryable();

            if (!string.IsNullOrEmpty(keywords) && !string.IsNullOrEmpty(orderDate) )
           {
            DateTime parsedOrderDate;
            if (DateTime.TryParseExact(orderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedOrderDate))
            {
                result = result.Where(m => (m.order_date.Date == parsedOrderDate.Date) && (m.Customer.customer_name.Contains(keywords) || m.order_no.Contains(keywords)));
            }
            }else if (!string.IsNullOrEmpty(keywords) )
            {
                result = result.Where(m => m.Customer.customer_name.Contains(keywords) || m.order_no.Contains(keywords));
            }else if (!string.IsNullOrEmpty(orderDate))
            {
                DateTime parsedOrderDate;
                if (DateTime.TryParseExact(orderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedOrderDate))
                {
                    result = result.Where(m => m.order_date.Date == parsedOrderDate.Date);
                }
            }

            var data = result.Select(order => new GridSalesOrderViewModel
            {
                order_no = order.order_no,
                order_date = order.order_date.ToString("dd/MM/yyyy"),
                customer = order.Customer.customer_name
            }).ToList();

            // Create Excel package using EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sales Orders");

                // Add header row
                worksheet.Cells[1, 1].Value = "No";
                worksheet.Cells[1, 2].Value = "Order No";
                worksheet.Cells[1, 3].Value = "Order Date";
                worksheet.Cells[1, 4].Value = "Customer";

                // Fill data
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = row - 1;
                    worksheet.Cells[row, 2].Value = item.order_no;
                    worksheet.Cells[row, 3].Value = item.order_date;
                    worksheet.Cells[row, 4].Value = item.customer;
                    row++;
                }

                // Set the content type for Excel file
                var fileBytes = package.GetAsByteArray();

                // Return the file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesOrders.xlsx");
            }
        }
        catch (Exception ex)
        {
            // Handle any errors (e.g., log them) and return an error view or message
            return BadRequest("Error generating Excel file: " + ex.Message);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

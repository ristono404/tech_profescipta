using Newtonsoft.Json;

namespace profescipta.Models;

public class GridSalesOrderViewModel
{
    public long so_order_id { get; set; }
    public string order_no { get; set; }
    public string order_date { get; set; }
    public string customer { get; set; }
}

public class SalesOrderViewModel
{
    public long so_order_id { get; set; }
    public string order_no { get; set; }
    public string order_date { get; set; }
    public int com_customer_id { get; set; }
    public string address { get; set; }
}

public class ItemViewModel
{
    public long so_item_id { get; set; }
    public string item_name { get; set; }
    public int quantity { get; set; }
    public double price { get; set; }
    public double total { get; set; }
}

public class CreateItemViewModel
{
    public long so_item_id { get; set; }
    public long so_order_id { get; set; }
    public string item_name { get; set; }
    public int quantity { get; set; }
    public double price { get; set; }
}

public class CreateOrderViewModel
{
    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public int ComCustomerId { get; set; }
    public string Address { get; set; }
    public List<CreateItemViewModel> Items { get; set; }
}

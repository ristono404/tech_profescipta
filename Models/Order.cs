using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace profescipta.Models;


[Table("SO_ITEM")]
public class Item
{
    public long so_item_id { get; set; }
    public long so_order_id { get; set; }
    public string item_name { get; set; }
    public int quantity { get; set; }
    public double price { get; set; }

    // Define foreign key for relationship
    [ForeignKey("so_order_id")]
    [JsonIgnore]  // Prevent circular reference during JSON serialization
    public Order Order { get; set; }
}

[Table("SO_ORDER")]
public class Order
{
    public long so_order_id { get; set; }
    public string order_no { get; set; }
    public DateTime order_date { get; set; }
    public int com_customer_id { get; set; }
    public string address { get; set; }

    // Navigation property
    [ForeignKey("com_customer_id")]
    public Customer Customer { get; set; }

    // Add [JsonIgnore] to avoid circular reference in Customer -> Order relationship
    [JsonIgnore]
    public ICollection<Item> Items { get; set; }
}

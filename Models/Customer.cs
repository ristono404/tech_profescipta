using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace profescipta.Models;

[Table("COM_CUSTOMER")]
public class Customer
{
    [Key]
    public int com_customer_id { get; set; }
    public string customer_name { get; set; }
}

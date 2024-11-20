using Microsoft.EntityFrameworkCore;
using profescipta.Models;

public class AppDbContext : DbContext, IMyDBContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

     public DbSet<Order> Orders { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the primary key for Item
        modelBuilder.Entity<Item>()
            .HasKey(i => i.so_item_id);  // Set SO_ITEM_ID as the primary key for Item
        
        // Configure the primary key for Order
        modelBuilder.Entity<Order>()
            .HasKey(o => o.so_order_id);  // Set SO_ORDER_ID as the primary key for Order

        // Configure the primary key for Customer (assuming you have a Customer class)
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.com_customer_id);  // Set COM_CUSTOMER_ID as the primary key for Customer

        // Define foreign key relationship between Item and Order
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Order)            // Each Item has one Order
            .WithMany(o => o.Items)          // Each Order has many Items
            .HasForeignKey(i => i.so_order_id); // Foreign key in Item for SO_ORDER_ID

        // Define foreign key relationship between Order and Customer
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)            // Each Order has one Customer
            .WithMany()                         // Each Customer can have many Orders
            .HasForeignKey(o => o.com_customer_id); // Foreign key in Order for COM_CUSTOMER_ID

        // Optional: Set default values if necessary
        modelBuilder.Entity<Order>()
            .Property(o => o.order_date)
            .HasDefaultValueSql("GETDATE()");  // Set a default value for ORDER_DATE

        modelBuilder.Entity<Item>()
            .Property(i => i.quantity)
            .HasDefaultValue(-99);  // Set a default value for QUANTITY

        // Optional: Configure additional constraints
        modelBuilder.Entity<Item>()
            .Property(i => i.item_name)
            .HasMaxLength(100); // Set maximum length for ITEM_NAME
    }

}
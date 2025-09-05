namespace StoreAPI.Models.Entities;

public class OrderProduct
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public double Amount { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; }
}

// PK Compuestas
namespace StoreAPI.Models.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    
    public int StoreId { get; set; }
    public Store Store { get; set; }
}
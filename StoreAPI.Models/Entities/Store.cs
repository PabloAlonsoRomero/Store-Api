namespace StoreAPI.Models.Entities;

public class Store
{
    public int Id { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public List<Product> Products { get; set; }
}
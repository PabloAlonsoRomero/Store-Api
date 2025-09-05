using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreAPI.Models.Entities;

public class Invoice
{
    public int Id { get; set; }
    
    [Required]
    public string InvoiceNumber { get; set; }
    
    [Required]
    public DateTime IssueDate { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public double Subtotal { get; set; }
    public double Tax { get; set; }
    public double Total { get; set; }
    
    [Required]
    public string Currency { get; set; }
    
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    
    [Required]
    public string BillingName { get; set; }
    
    public string BillingAddress { get; set; }
    
    [EmailAddress]
    public string BillingEmail { get; set; }
    
    public string TaxId{ get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key
    public int OrderId { get; set; }
}
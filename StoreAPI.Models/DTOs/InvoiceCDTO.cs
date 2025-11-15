namespace StoreAPI.Models.DTOs;

public class InvoiceCDTO
{
    public string InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double Subtotal { get; set; }
    public double Tax { get; set; }
    public double Total { get; set; }
    public string Currency { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string BillingName { get; set; }
    public string BillingAddress { get; set; }
    public string BillingEmail { get; set; }
    public string TaxId { get; set; }
    public int OrderId { get; set; }
}
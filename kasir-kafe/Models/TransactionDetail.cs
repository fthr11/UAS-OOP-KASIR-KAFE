namespace kasirkafe.Models;

public class TransactionDetail
{
    public int TransactionDetailId { get; set; }
    public int TransactionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    
    public virtual Transaction? Transaction { get; set; }
    public virtual Product? Product { get; set; }
}
namespace kasirkafe.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public required string TransactionCode { get; set; }
    public int UserId { get; set; }
    public string? CustomerName { get; set; } 
    public decimal TotalAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    
    // Navigation properties - JANGAN REQUIRED untuk avoid circular reference
    public virtual User? User { get; set; }
    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    
    public void CalculateTotal()
    {
        TotalAmount = TransactionDetails.Sum(d => d.Subtotal);
    }
    
    public void CalculateChange()
    {
        ChangeAmount = PaymentAmount - TotalAmount;
    }
}
public class Transaction
{
    public int TransactionId { get; set; }
    public required string TransactionCode { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public DateTime TransactionDate { get; set; }

    public virtual required User User { get; set; }
    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new HashSet<TransactionDetail>();
}
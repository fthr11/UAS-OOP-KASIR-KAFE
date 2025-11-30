public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new HashSet<TransactionDetail>();
}
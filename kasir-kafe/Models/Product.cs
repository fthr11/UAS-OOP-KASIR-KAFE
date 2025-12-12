namespace kasirkafe.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Category { get; set; }
    public required string? Image { get; set; }

    [NotMapped] 
    public IFormFile? ImageFile { get; set; } // Untuk foto disimpan ke local storage
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new HashSet<TransactionDetail>();
}
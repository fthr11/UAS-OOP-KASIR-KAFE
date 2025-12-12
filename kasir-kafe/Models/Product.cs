namespace kasirkafe.Models;
using System.ComponentModel.DataAnnotations.Schema;


public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Category { get; set; }
    public string? Image { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; } // Untuk foto disimpan ke local storage
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new HashSet<TransactionDetail>();
}

public class FoodProduct : Product
{
    public string? JenisMakanan { get; set; }   
}

public class DrinkProduct : Product
{
    public string? JenisMinuman { get; set; } 
}


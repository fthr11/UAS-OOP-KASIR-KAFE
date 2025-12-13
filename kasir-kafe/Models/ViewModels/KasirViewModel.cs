using kasirkafe.Models;
namespace kasirkafe.ViewModels;

public class KasirViewModel
{
    public string? CustomerName { get; set; }
    public decimal PaymentAmount { get; set; }

    public List<Product> Products { get; set; } = new();
    public List<TransactionDetail> Cart { get; set; } = new();

    public decimal TotalAmount => Cart.Sum(c => c.Subtotal);
    public decimal ChangeAmount => PaymentAmount - TotalAmount;
}

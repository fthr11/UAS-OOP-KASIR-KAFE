using kasirkafe.Models;

namespace kasirkafe.Models.ViewModels;

public class TransactionViewModel
{
    public List<Product> AvailableProducts { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();

    public decimal TotalAmount =>
        CartItems.Sum(c => c.Subtotal);

    public decimal PaymentAmount { get; set; }

    public decimal ChangeAmount =>
        PaymentAmount - TotalAmount;
}

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}
namespace WebShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Kontaktoplysninger
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        // Leveringsadresse
        public string Address { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = "DK";

        // Andet
        public string? Comment { get; set; }
        public DateTime TermsAcceptedAt { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
namespace WebShop.Dtos
{
    public class ProductInCart
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
    }
}

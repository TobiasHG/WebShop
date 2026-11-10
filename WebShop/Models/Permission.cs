namespace WebShop.Models
{
    public static class Permissions
    {
        public const string CanResetPassword = "reset.password";

        public const string CanManipulateProducts = "products.manipulate";

        public const string CanPlaceOrder = "order.place";

        public static readonly IReadOnlyList<string> All =
        [
            CanResetPassword,
            CanManipulateProducts,
            CanPlaceOrder,
        ];
    }
}
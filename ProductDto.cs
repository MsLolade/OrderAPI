namespace OrderAPI
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }
    public class OrderDto
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

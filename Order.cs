using System.ComponentModel.DataAnnotations;

namespace OrderAPI
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }
}

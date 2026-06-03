using ECommerce.DAL;

namespace ECommerce.DAL
{
    public class Category : IAuditEntity
    {
            public int Id { get; set; }
            public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; } = false; 
        public List<Product> Products { get; set; } = new();
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
    }
}


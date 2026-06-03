using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.DAL
{
    public class Cart : IAuditEntity
    {
        public int Id { get; set; }

        // FK to ApplicationUser (string because IdentityUser uses string PK)
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        // Navigation
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        //public DateTime CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }

    }

    public class CartItem : IAuditEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        /// <summary>
        /// Price snapshot at time of adding to cart.
        /// Protects against price changes affecting cart display.
        /// </summary>
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}

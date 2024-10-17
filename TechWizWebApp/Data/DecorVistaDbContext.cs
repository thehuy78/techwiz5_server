using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Data
{
    public class DecorVistaDbContext : DbContext
    {
        public DecorVistaDbContext(DbContextOptions<DecorVistaDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Functionality> Functionalities { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryDetails> GalleryDetails { get; set; }
        public DbSet<InteriorDesigner> InteriorDesigners { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Subcribe> Subcribes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<VariantAttribute> VariantAttributes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Notification> Notifications { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                 .HasOne(u => u.userdetails)
                 .WithOne(ud => ud.User)
                 .HasForeignKey<UserDetails>(ud => ud.user_id);

            modelBuilder.Entity<User>()
                .HasOne(u => u.interiordesigner)
                .WithOne(id => id.user)
                .HasForeignKey<InteriorDesigner>(id => id.user_id);

            //many to many consutation with user and designer
            modelBuilder.Entity<Consultation>()
               .HasOne(c => c.user)
               .WithMany(u => u.consultations)
               .HasForeignKey(c => c.user_id)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Consultation>()
                .HasOne(c => c.interior_designer)
                .WithMany(d => d.consultations)
                .HasForeignKey(c => c.designer_id)
                .OnDelete(DeleteBehavior.NoAction);

            //many to many review with product and designer
            modelBuilder.Entity<Review>()
                .HasOne(r => r.user)
                .WithMany(u => u.reviews)
                .HasForeignKey(r => r.user_id)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Review>()
              .HasOne(r => r.product)
              .WithMany(p => p.reviews)
              .HasForeignKey(r => r.product_id)
              .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Review>()
             .HasOne(r => r.Consultation)
             .WithOne(c => c.review)
             .HasForeignKey<Review>(r => r.id_booking)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
              .HasOne(r => r.product)
              .WithMany(id => id.reviews)
              .HasForeignKey(r => r.product_id)
              .OnDelete(DeleteBehavior.NoAction);

            // many to many cart with product and user
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.user)
                .WithMany(u => u.carts)
                .HasForeignKey(c => c.user_id)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.product)
                .WithMany(p => p.carts)
                .HasForeignKey(c => c.product_id)
                .OnDelete(DeleteBehavior.NoAction);

            // many to many cart with variant and user
            modelBuilder.Entity<Cart>()
                 .HasOne(c => c.user)
                 .WithMany(u => u.carts)
                 .HasForeignKey(c => c.user_id)
                 .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.variant)
                .WithMany(p => p.carts)
                .HasForeignKey(c => c.variant_id)
                .OnDelete(DeleteBehavior.NoAction);

            // many to many galarydetails with gallery and product
            modelBuilder.Entity<GalleryDetails>()
                .HasOne(gd => gd.gallery)
                .WithMany(g => g.galleryDetails)
                .HasForeignKey(gd => gd.gallery_id);

            modelBuilder.Entity<GalleryDetails>()
                .HasOne(gd => gd.product)
                .WithMany(p => p.galleryDetails)
                .HasForeignKey(gd => gd.product_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.functionality)
                .WithMany(f => f.products)
                .HasForeignKey(p => p.functionality_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.variants).WithOne(p => p.product)
                .HasForeignKey(p => p.productid)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
              .HasMany(p => p.images).WithOne(p => p.product)
              .HasForeignKey(p => p.productid)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Variant>()
                .HasMany(p => p.variantattributes)
                .WithOne(p => p.variant)
                .HasForeignKey(p => p.variantid)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.room_type)
                .WithMany(rt => rt.galleries)
                .HasForeignKey(g => g.room_type_id)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Gallery>()
              .HasOne(b => b.interior_designer)
              .WithMany(id => id.galleries)
              .HasForeignKey(b => b.interior_designer_id)
              .OnDelete(DeleteBehavior.NoAction);
          
            modelBuilder.Entity<Order>()
                .HasOne(o => o.user)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.user_id);

            //many to many subcribe gallery and user
            modelBuilder.Entity<Subcribe>()
                .HasOne(s => s.gallery)
                .WithMany(g => g.subcribes)
                .HasForeignKey(s => s.gallery_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Subcribe>()
                .HasOne(s => s.user)
                .WithMany(u => u.subcribes)
                .HasForeignKey(s => s.user_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.interior_designer)
                .WithMany(id => id.blogs)
                .HasForeignKey(b => b.interior_designer_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.images).WithOne(i => i.product).HasForeignKey(i => i.productid).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.order)
                .WithMany(o => o.order_details)
                .HasForeignKey(od => od.order_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.variant)
                .WithMany(v => v.order_details)
                .HasForeignKey(od => od.variant_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
               .HasOne(b => b.user)
               .WithMany(id => id.notifications)
               .HasForeignKey(b => b.user_id)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Story>()
                .HasOne(b => b.interior_designer)
                .WithMany(id => id.stories)
                .HasForeignKey(b => b.interior_designer_id)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Review>()
             .HasOne(r => r.Consultation)
             .WithOne(c => c.review)
             .HasForeignKey<Review>(r => r.id_booking)
             .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }
    }
}

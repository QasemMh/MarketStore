using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MarketStore.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategorySection> CategorySections { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreCategory> StoreCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserMessage> UserMessages { get; set; }
        public virtual DbSet<WebsiteInfo> WebsiteInfos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TAH14_USER30")
                .HasAnnotation("Relational:Collation", "USING_NLS_COMP");

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("ADDRESS");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS2");

                entity.Property(e => e.Address3)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS3");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CITY");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORY");

                entity.HasIndex(e => e.Name, "SYS_C00220502")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<CategorySection>(entity =>
            {
                entity.ToTable("CATEGORY_SECTION");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Title)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.ToTable("CREDIT_CARD");

                entity.HasIndex(e => e.CustomerId, "SYS_C00220477")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Balance)
                    .HasColumnType("NUMBER(7,2)")
                    .HasColumnName("BALANCE")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CardNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CARD_NUMBER");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(11)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.ExpiryMonth)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("EXPIRY_MONTH")
                    .IsFixedLength(true);

                entity.Property(e => e.ExpiryYear)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("EXPIRY_YEAR")
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.SecurityCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SECURITY_CODE")
                    .IsFixedLength(true);

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.CreditCard)
                    .HasForeignKey<CreditCard>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("CARD_CUSTOMER_ID");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.HasIndex(e => e.AddressId, "SYS_C00220468")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.AddressId)
                    .HasPrecision(11)
                    .HasColumnName("ADDRESS_ID");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("MIDDLE_NAME");

                entity.HasOne(d => d.Address)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.AddressId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("CUSTOMER_ADDRESS_ID");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(11)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.OrderDate)
                    .HasPrecision(6)
                    .HasColumnName("ORDER_DATE")
                    .HasDefaultValueSql("current_timestamp");

                entity.Property(e => e.Status)
                    .HasPrecision(1)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("0 ");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("ORDER_CUSTOMER_ID");
            });

            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.ToTable("ORDER_LINE");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.OrderId)
                    .HasPrecision(11)
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.ProductId)
                    .HasPrecision(11)
                    .HasColumnName("PRODUCT_ID");

                entity.Property(e => e.Quantity)
                    .HasPrecision(3)
                    .HasColumnName("QUANTITY");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("ORDER_ORDERLINE_ID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("PRODUCT_ORDERLINE_ID");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryId)
                    .HasPrecision(11)
                    .HasColumnName("CATEGORY_ID");

                entity.Property(e => e.Cost)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("COST");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Quantitiy)
                    .HasPrecision(3)
                    .HasColumnName("QUANTITIY")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.StoreId)
                    .HasPrecision(11)
                    .HasColumnName("STORE_ID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("PRODUCT_CATEGORY_ID");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("PRODUCT_STORE_ID");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("PRODUCT_IMAGES");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.ProductId)
                    .HasPrecision(11)
                    .HasColumnName("PRODUCT_ID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("PRODUCT_IMAGES_ID");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("REVIEWS");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("JOB_TITLE");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Review1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REVIEW");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<Slider>(entity =>
            {
                entity.ToTable("SLIDER");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Title)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("STORES");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryId)
                    .HasPrecision(11)
                    .HasColumnName("CATEGORY_ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.StoreCategory)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("STORE_CATEGORY_ID");
            });

            modelBuilder.Entity<StoreCategory>(entity =>
            {
                entity.ToTable("STORE_CATEGORY");

                entity.HasIndex(e => e.Name, "SYS_C00220497")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.HasIndex(e => e.Username, "SYS_C00220487")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "SYS_C00220488")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "SYS_C00220489")
                    .IsUnique();

                entity.HasIndex(e => e.CustomerId, "SYS_C00220490")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(11)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.HashPassword)
                    .IsRequired()
                    .HasMaxLength(260)
                    .IsUnicode(false)
                    .HasColumnName("HASH_PASSWORD");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");

                entity.Property(e => e.RoleId)
                    .HasPrecision(11)
                    .HasColumnName("ROLE_ID");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("USERNAME");

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("USERS_CUSTOMER_ID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USERS_ROLE_ID");
            });

            modelBuilder.Entity<UserMessage>(entity =>
            {
                entity.ToTable("USER_MESSAGE");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.Email)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Message)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Subject)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("SUBJECT");
            });

            modelBuilder.Entity<WebsiteInfo>(entity =>
            {
                entity.ToTable("WEBSITE_INFO");

                entity.Property(e => e.Id)
                    .HasPrecision(11)
                    .HasColumnName("ID");

                entity.Property(e => e.BrefDescription)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("BREF_DESCRIPTION");

                entity.Property(e => e.Email)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.IogoImage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IOGO_IMAGE");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.LogoName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOGO_NAME");

                entity.Property(e => e.Phone)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");
            });

            modelBuilder.HasSequence("DEPT_SEQ");

            modelBuilder.HasSequence("DEPT_SEQ2").IncrementsBy(10);

            modelBuilder.HasSequence("EMPLOYEE_SEQ");

            modelBuilder.HasSequence("UPDATE_SEQUENCE").IncrementsBy(10);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

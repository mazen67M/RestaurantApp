using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Infrastructure.Data.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(r => r.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(r => r.DescriptionAr).HasMaxLength(1000);
        builder.Property(r => r.DescriptionEn).HasMaxLength(1000);
        builder.Property(r => r.LogoUrl).HasMaxLength(500);
        builder.Property(r => r.CoverImageUrl).HasMaxLength(500);
        builder.Property(r => r.PrimaryColor).HasMaxLength(20);
        builder.Property(r => r.SecondaryColor).HasMaxLength(20);
        builder.Property(r => r.Phone).HasMaxLength(20);
        builder.Property(r => r.Email).HasMaxLength(200);
        builder.Property(r => r.Website).HasMaxLength(200);

        builder.HasMany(r => r.Branches)
            .WithOne(b => b.Restaurant)
            .HasForeignKey(b => b.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.MenuCategories)
            .WithOne(c => c.Restaurant)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(b => b.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(b => b.AddressAr).IsRequired().HasMaxLength(500);
        builder.Property(b => b.AddressEn).IsRequired().HasMaxLength(500);
        builder.Property(b => b.Phone).HasMaxLength(20);

        builder.HasMany(b => b.Orders)
            .WithOne(o => o.Branch)
            .HasForeignKey(o => o.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.AcceptingOrders);
    }
}

public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(c => c.DescriptionAr).HasMaxLength(500);
        builder.Property(c => c.DescriptionEn).HasMaxLength(500);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);

        builder.HasMany(c => c.MenuItems)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(i => i.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(i => i.DescriptionAr).HasMaxLength(1000);
        builder.Property(i => i.DescriptionEn).HasMaxLength(1000);
        builder.Property(i => i.ImageUrl).HasMaxLength(500);

        builder.HasMany(i => i.AddOns)
            .WithOne(a => a.MenuItem)
            .HasForeignKey(a => a.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.CategoryId);
        builder.HasIndex(i => i.NameEn);
        builder.HasIndex(i => i.IsAvailable);
        builder.HasIndex(i => i.IsPopular);
    }
}

public class MenuItemAddOnConfiguration : IEntityTypeConfiguration<MenuItemAddOn>
{
    public void Configure(EntityTypeBuilder<MenuItemAddOn> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(a => a.NameEn).IsRequired().HasMaxLength(200);
    }
}

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Label).IsRequired().HasMaxLength(50);
        builder.Property(a => a.AddressLine).IsRequired().HasMaxLength(500);
        builder.Property(a => a.BuildingName).HasMaxLength(200);
        builder.Property(a => a.Floor).HasMaxLength(20);
        builder.Property(a => a.Apartment).HasMaxLength(20);
        builder.Property(a => a.Landmark).HasMaxLength(200);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(20);
        builder.Property(o => o.DeliveryAddressLine).HasMaxLength(500);
        builder.Property(o => o.DeliveryNotes).HasMaxLength(500);
        builder.Property(o => o.CustomerNotes).HasMaxLength(500);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
        builder.HasIndex(o => o.UserId);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.MenuItemNameAr).IsRequired().HasMaxLength(200);
        builder.Property(i => i.MenuItemNameEn).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.HasOne(i => i.MenuItem)
            .WithMany()
            .HasForeignKey(i => i.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.OrderItemAddOns)
            .WithOne(a => a.OrderItem)
            .HasForeignKey(a => a.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderItemAddOnConfiguration : IEntityTypeConfiguration<OrderItemAddOn>
{
    public void Configure(EntityTypeBuilder<OrderItemAddOn> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(a => a.NameEn).IsRequired().HasMaxLength(200);

        builder.HasOne(a => a.MenuItemAddOn)
            .WithMany()
            .HasForeignKey(a => a.MenuItemAddOnId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
        builder.Property(u => u.PreferredLanguage).HasMaxLength(10).HasDefaultValue("ar");

        builder.HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Code).IsRequired().HasMaxLength(50);
        builder.Property(o => o.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(o => o.NameEn).IsRequired().HasMaxLength(200);
        
        builder.HasIndex(o => o.Code).IsUnique();
        builder.HasIndex(o => o.StartDate);
        builder.HasIndex(o => o.EndDate);
        builder.HasIndex(o => o.IsActive);
    }
}

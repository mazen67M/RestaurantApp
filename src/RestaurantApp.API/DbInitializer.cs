using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed roles
        string[] roles = { "Customer", "Cashier", "Admin", "Driver" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
            }
        }

        // Seed admin user
        var adminEmail = "admin@restaurant.com";
        var adminPassword = "AdminSecure@123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            Console.WriteLine("[DbInitializer] Creating new admin user...");
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true,
                IsActive = true,
                PreferredLanguage = "en"
            };
            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            Console.WriteLine($"[DbInitializer] Admin create result: {createResult.Succeeded}");
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    Console.WriteLine($"[DbInitializer] Error: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("[DbInitializer] Updating existing admin user...");
            // Force update password and ensure email is confirmed
            var hasPassword = await userManager.HasPasswordAsync(adminUser);
            if (hasPassword)
            {
                var removeResult = await userManager.RemovePasswordAsync(adminUser);
                Console.WriteLine($"[DbInitializer] Remove password result: {removeResult.Succeeded}");
            }
            var addResult = await userManager.AddPasswordAsync(adminUser, adminPassword);
            Console.WriteLine($"[DbInitializer] Add password result: {addResult.Succeeded}");
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    Console.WriteLine($"[DbInitializer] Error: {error.Description}");
                }
            }
            
            adminUser.EmailConfirmed = true;
            adminUser.IsActive = true;
            await userManager.UpdateAsync(adminUser);
            Console.WriteLine("[DbInitializer] Admin user updated successfully");
        }
        
        // Ensure admin role is assigned
        if (!await userManager.IsInRoleAsync(adminUser!, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser!, "Admin");
            Console.WriteLine("[DbInitializer] Admin role assigned");
        }

        // Seed restaurant if not exists
        if (!await context.Restaurants.AnyAsync())
        {
            var restaurant = new Restaurant
            {
                NameAr = "مطعم الذواق",
                NameEn = "Al Thawaq Restaurant",
                DescriptionAr = "أفضل المأكولات العربية والعالمية",
                DescriptionEn = "Best Arabic and International Cuisine",
                PrimaryColor = "#FF5722",
                SecondaryColor = "#FFC107",
                Phone = "+971501234567",
                Email = "info@restaurant.com",
                IsActive = true
            };

            context.Restaurants.Add(restaurant);
            await context.SaveChangesAsync();

            // Add branches
            var branch1 = new Branch
            {
                RestaurantId = restaurant.Id,
                NameAr = "فرع دبي مول",
                NameEn = "Dubai Mall Branch",
                AddressAr = "دبي مول، الطابق الأرضي",
                AddressEn = "Dubai Mall, Ground Floor",
                Latitude = 25.1972m,
                Longitude = 55.2797m,
                Phone = "+971501234567",
                DeliveryRadiusKm = 10,
                MinOrderAmount = 50,
                DeliveryFee = 10,
                FreeDeliveryThreshold = 100,
                DefaultPreparationTimeMinutes = 30,
                OpeningTime = new TimeSpan(10, 0, 0),
                ClosingTime = new TimeSpan(23, 0, 0),
                IsActive = true,
                AcceptingOrders = true
            };

            var branch2 = new Branch
            {
                RestaurantId = restaurant.Id,
                NameAr = "فرع مارينا",
                NameEn = "Marina Branch",
                AddressAr = "دبي مارينا، برج الأميرة",
                AddressEn = "Dubai Marina, Princess Tower",
                Latitude = 25.0920m,
                Longitude = 55.1413m,
                Phone = "+971501234568",
                DeliveryRadiusKm = 8,
                MinOrderAmount = 40,
                DeliveryFee = 8,
                FreeDeliveryThreshold = 80,
                DefaultPreparationTimeMinutes = 25,
                OpeningTime = new TimeSpan(11, 0, 0),
                ClosingTime = new TimeSpan(24, 0, 0),
                IsActive = true,
                AcceptingOrders = true
            };

            context.Branches.AddRange(branch1, branch2);
            await context.SaveChangesAsync();

            // Add menu categories
            var categories = new[]
            {
                new MenuCategory
                {
                    RestaurantId = restaurant.Id,
                    NameAr = "المقبلات",
                    NameEn = "Appetizers",
                    DescriptionAr = "مقبلات شهية لبداية رائعة",
                    DescriptionEn = "Delicious appetizers for a great start",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuCategory
                {
                    RestaurantId = restaurant.Id,
                    NameAr = "الأطباق الرئيسية",
                    NameEn = "Main Dishes",
                    DescriptionAr = "أطباق رئيسية متنوعة",
                    DescriptionEn = "Variety of main dishes",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuCategory
                {
                    RestaurantId = restaurant.Id,
                    NameAr = "البرجر",
                    NameEn = "Burgers",
                    DescriptionAr = "برجر طازج ولذيذ",
                    DescriptionEn = "Fresh and delicious burgers",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MenuCategory
                {
                    RestaurantId = restaurant.Id,
                    NameAr = "المشروبات",
                    NameEn = "Beverages",
                    DescriptionAr = "مشروبات منعشة",
                    DescriptionEn = "Refreshing beverages",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new MenuCategory
                {
                    RestaurantId = restaurant.Id,
                    NameAr = "الحلويات",
                    NameEn = "Desserts",
                    DescriptionAr = "حلويات شرقية وغربية",
                    DescriptionEn = "Eastern and Western desserts",
                    DisplayOrder = 5,
                    IsActive = true
                }
            };

            context.MenuCategories.AddRange(categories);
            await context.SaveChangesAsync();

            // Add menu items
            var appetizersCategory = categories[0];
            var mainCategory = categories[1];
            var burgersCategory = categories[2];
            var beveragesCategory = categories[3];
            var dessertsCategory = categories[4];

            var menuItems = new List<MenuItem>
            {
                // Appetizers
                new MenuItem
                {
                    CategoryId = appetizersCategory.Id,
                    NameAr = "حمص",
                    NameEn = "Hummus",
                    DescriptionAr = "حمص كريمي مع زيت الزيتون",
                    DescriptionEn = "Creamy hummus with olive oil",
                    Price = 25,
                    IsAvailable = true,
                    IsPopular = true,
                    PreparationTimeMinutes = 10
                },
                new MenuItem
                {
                    CategoryId = appetizersCategory.Id,
                    NameAr = "فتوش",
                    NameEn = "Fattoush",
                    DescriptionAr = "سلطة لبنانية تقليدية مع الخبز المحمص",
                    DescriptionEn = "Traditional Lebanese salad with toasted bread",
                    Price = 30,
                    IsAvailable = true,
                    PreparationTimeMinutes = 10
                },
                new MenuItem
                {
                    CategoryId = appetizersCategory.Id,
                    NameAr = "سمبوسة",
                    NameEn = "Samosa",
                    DescriptionAr = "سمبوسة محشية بالخضار أو اللحم",
                    DescriptionEn = "Samosa stuffed with vegetables or meat",
                    Price = 20,
                    IsAvailable = true,
                    PreparationTimeMinutes = 15
                },

                // Main Dishes
                new MenuItem
                {
                    CategoryId = mainCategory.Id,
                    NameAr = "مشاوي مشكلة",
                    NameEn = "Mixed Grill",
                    DescriptionAr = "تشكيلة من اللحوم المشوية مع الأرز",
                    DescriptionEn = "Selection of grilled meats with rice",
                    Price = 120,
                    IsAvailable = true,
                    IsPopular = true,
                    PreparationTimeMinutes = 25
                },
                new MenuItem
                {
                    CategoryId = mainCategory.Id,
                    NameAr = "كبسة لحم",
                    NameEn = "Lamb Kabsa",
                    DescriptionAr = "أرز بالبهارات مع لحم الضأن",
                    DescriptionEn = "Spiced rice with lamb",
                    Price = 85,
                    IsAvailable = true,
                    PreparationTimeMinutes = 30
                },
                new MenuItem
                {
                    CategoryId = mainCategory.Id,
                    NameAr = "سمك مشوي",
                    NameEn = "Grilled Fish",
                    DescriptionAr = "سمك طازج مشوي مع الخضار",
                    DescriptionEn = "Fresh grilled fish with vegetables",
                    Price = 95,
                    IsAvailable = true,
                    PreparationTimeMinutes = 25
                },

                // Burgers
                new MenuItem
                {
                    CategoryId = burgersCategory.Id,
                    NameAr = "برجر كلاسيك",
                    NameEn = "Classic Burger",
                    DescriptionAr = "برجر لحم مع الجبن والخضار",
                    DescriptionEn = "Beef burger with cheese and vegetables",
                    Price = 45,
                    IsAvailable = true,
                    IsPopular = true,
                    PreparationTimeMinutes = 15
                },
                new MenuItem
                {
                    CategoryId = burgersCategory.Id,
                    NameAr = "برجر دجاج",
                    NameEn = "Chicken Burger",
                    DescriptionAr = "برجر دجاج مقرمش",
                    DescriptionEn = "Crispy chicken burger",
                    Price = 40,
                    IsAvailable = true,
                    PreparationTimeMinutes = 15
                },

                // Beverages
                new MenuItem
                {
                    CategoryId = beveragesCategory.Id,
                    NameAr = "عصير برتقال طازج",
                    NameEn = "Fresh Orange Juice",
                    DescriptionAr = "عصير برتقال طبيعي",
                    DescriptionEn = "Natural orange juice",
                    Price = 18,
                    IsAvailable = true,
                    PreparationTimeMinutes = 5
                },
                new MenuItem
                {
                    CategoryId = beveragesCategory.Id,
                    NameAr = "ليمون بالنعناع",
                    NameEn = "Lemon Mint",
                    DescriptionAr = "ليمون منعش مع النعناع",
                    DescriptionEn = "Refreshing lemon with mint",
                    Price = 15,
                    IsAvailable = true,
                    IsPopular = true,
                    PreparationTimeMinutes = 5
                },

                // Desserts
                new MenuItem
                {
                    CategoryId = dessertsCategory.Id,
                    NameAr = "كنافة",
                    NameEn = "Kunafa",
                    DescriptionAr = "كنافة نابلسية بالجبن",
                    DescriptionEn = "Nabulsi kunafa with cheese",
                    Price = 35,
                    IsAvailable = true,
                    IsPopular = true,
                    PreparationTimeMinutes = 10
                },
                new MenuItem
                {
                    CategoryId = dessertsCategory.Id,
                    NameAr = "أم علي",
                    NameEn = "Um Ali",
                    DescriptionAr = "حلوى مصرية تقليدية بالكريمة والمكسرات",
                    DescriptionEn = "Traditional Egyptian dessert with cream and nuts",
                    Price = 30,
                    IsAvailable = true,
                    PreparationTimeMinutes = 10
                }
            };

            context.MenuItems.AddRange(menuItems);
            await context.SaveChangesAsync();

            // Add add-ons to burgers
            var classicBurger = menuItems.First(i => i.NameEn == "Classic Burger");
            var chickenBurger = menuItems.First(i => i.NameEn == "Chicken Burger");

            var burgerAddOns = new[]
            {
                new MenuItemAddOn { MenuItemId = classicBurger.Id, NameAr = "جبنة إضافية", NameEn = "Extra Cheese", Price = 5, IsAvailable = true },
                new MenuItemAddOn { MenuItemId = classicBurger.Id, NameAr = "بيض", NameEn = "Egg", Price = 5, IsAvailable = true },
                new MenuItemAddOn { MenuItemId = classicBurger.Id, NameAr = "بيكون", NameEn = "Bacon", Price = 8, IsAvailable = true },
                new MenuItemAddOn { MenuItemId = classicBurger.Id, NameAr = "مخلل", NameEn = "Pickles", Price = 3, IsAvailable = true },
                new MenuItemAddOn { MenuItemId = chickenBurger.Id, NameAr = "جبنة إضافية", NameEn = "Extra Cheese", Price = 5, IsAvailable = true },
                new MenuItemAddOn { MenuItemId = chickenBurger.Id, NameAr = "صوص حار", NameEn = "Spicy Sauce", Price = 3, IsAvailable = true },
            };

            context.MenuItemAddOns.AddRange(burgerAddOns);
            await context.SaveChangesAsync();
        }
    }
}

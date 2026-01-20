using RestaurantApp.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls with cookie support
builder.Services.AddHttpClient("RestaurantAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Create HttpClient that forwards cookies from HttpContext
builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("RestaurantAPI");
    
    // Forward cookies from current request (works during initial render)
    if (httpContext?.Request.Headers.ContainsKey("Cookie") == true)
    {
        var cookies = httpContext.Request.Headers["Cookie"].ToString();
        if (!string.IsNullOrEmpty(cookies))
        {
            client.DefaultRequestHeaders.Add("Cookie", cookies);
        }
    }
    
    return client;
});

builder.Services.AddHttpContextAccessor();

// Add API services
builder.Services.AddScoped<RestaurantApp.Web.Services.OrderApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.CategoryApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.MenuApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.OfferApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.ReviewApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.LoyaltyApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.DeliveryApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.BranchApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.UserApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.ReportApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.RestaurantApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.MediaApiService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.NotificationService>();
builder.Services.AddScoped<RestaurantApp.Web.Services.SidebarService>();


// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.LogoutPath = "/admin/logout";
        options.Cookie.Name = "RestaurantAdmin";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin", "SuperAdmin"));
});

// Add cascade authentication state
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.WithOrigins("http://localhost:*", "https://localhost:*")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFlutter");
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/admin/logout", async (HttpContext httpContext) =>
{
    try
    {
        // SECURITY: Call API to blacklist the JWT token
        var token = httpContext.User.FindFirst("AuthToken")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            var clientFactory = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("RestaurantAPI");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            // We fire and forget or wait briefly - don't block logout if API is down
            await client.PostAsync("/api/auth/logout", null);
        }
    }
    catch (Exception)
    {
        // Log error but proceed with cookie signout
    }

    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/admin/login");
});

app.Run();

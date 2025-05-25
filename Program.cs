using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var baglantiCumlesi = builder.Configuration.GetConnectionString("baglanti");
builder.Services.AddDbContext<ApplicationDbContext>(options=> options.UseSqlite(baglantiCumlesi));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Admin hesabı oluşturma
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    // Veritabanının mevcut olduğundan emin ol
    dbContext.Database.EnsureCreated();

    // Admin hesabını oluştur veya güncelle
    CreateOrUpdateAdminAccount(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

// Add session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Admin hesabı oluşturma veya güncelleme metodu
void CreateOrUpdateAdminAccount(ApplicationDbContext context)
{
    // Admin bilgileri
    string adminEmail = "necati@gmail.com";
    string adminPassword = "123456";
    string adminUsername = "admin";

    // Şifreyi hashle
    string hashedPassword = HashPassword(adminPassword);

    // Admin hesabının var olup olmadığını kontrol et
    var adminUser = context.Users.FirstOrDefault(u => u.Email == adminEmail);

    if (adminUser == null)
    {
        // Admin hesabı yoksa oluştur
        context.Users.Add(new User
        {
            Username = adminUsername,
            Email = adminEmail,
            Password = hashedPassword,
            FirstName = "Necati",
            LastName = "Admin",
            Role = "Admin",
            CreatedAt = DateTime.Now
        });
    }
    else
    {
        // Admin hesabı varsa güncelle
        adminUser.Password = hashedPassword;
        adminUser.Username = adminUsername;
        adminUser.Role = "Admin";
    }

    context.SaveChanges();
}

// Şifre hashleme metodu
string HashPassword(string password)
{
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

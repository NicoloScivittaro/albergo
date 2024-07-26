using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using albergo.DAO;
using Albergo.Services;
using Albergo.DAO; // Ensure the namespace for IAuthService is included

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configurazione dell'autenticazione
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
    });

// Configurazione delle policy di autorizzazione
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
    options.AddPolicy("ReceptionistPolicy", policy => policy.RequireRole("receptionist", "manager"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("manager"));
    options.AddPolicy("GeneralAccessPolicy", policy =>
        policy.RequireRole("admin", "receptionist", "manager"));
});

// Configurazione del servizio di gestione delle autenticazioni
builder.Services.AddScoped<IAuthService, AuthService>();

// Registrazione dei DAO con stringa di connessione
var connectionString = builder.Configuration.GetConnectionString("HotelDatabase");
builder.Services.AddScoped<IClienteDao>(provider => new ClienteDao(connectionString));
builder.Services.AddScoped<ICameraDao>(provider => new CameraDao(connectionString));
builder.Services.AddScoped<IServizioAggiuntivoDao>(provider => new ServizioDao(connectionString));
builder.Services.AddScoped<IPrenotazioneDao>(provider => new PrenotazioneDao(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ricerca",
    pattern: "{controller=Ricerca}/{action=Index}/{id?}");

app.Run();

using Application_Security_Project_v2;
using Application_Security_Project_v2.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyDbContext>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<CaptchaService>();

//Session Services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});

//SignInThings
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
	opt.Lockout.AllowedForNewUsers = true;
	opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
	opt.Lockout.MaxFailedAccessAttempts = 3;
}).AddEntityFrameworkStores<MyDbContext>();
builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
});

//GoogleCaptcha
builder.Services.Configure<GoogleCaptchaChain>(builder.Configuration.GetSection("GoogleCaptcha"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseStatusCodePagesWithRedirects("/Errors/{0}");
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

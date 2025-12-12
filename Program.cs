using Microsoft.EntityFrameworkCore;
using HrApi.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using HrBackend.Helpers;
using HrApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

//seed Super Admin 
async Task SeedSuperAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Create roles
    if (!await roleManager.RoleExistsAsync("SuperAdmin"))
    {
        await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
    }

    // Create Super Admin
    string email = "abanoub.edward97101@gmail.com";
    string password = "Admin@123";
    // 🔥 Add these fields to avoid NULL constraint violation


    var admin = await userManager.FindByEmailAsync(email);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Abanoub",
            LastName = "Edward"
        };
        await userManager.CreateAsync(admin, password);
        await userManager.AddToRoleAsync(admin, "SuperAdmin");
    }

}

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<HrContext>();
//builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddAutoMapper(Action<IMapperConfigurationExpression> configure);
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<EmployeeProfile>();  // add as many profiles as you have
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<HrContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWT>();
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.key))
    };
});
var app = builder.Build();
await SeedSuperAdminAsync(app);
//app.MapIdentityApi<IdentityUser>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
{
    options.DocumentPath = "/openapi/v1.json";
});
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);  

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

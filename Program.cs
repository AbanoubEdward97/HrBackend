using Microsoft.EntityFrameworkCore;
using HrApi.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using HrBackend.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HrBackend.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using HrBackend.Mapping;
using HrBackend.Services;
//seed Super Admin 
// async Task SeedSuperAdminAsync(WebApplication app)
// {
//     using var scope = app.Services.CreateScope();
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//     // Create roles
//     if (!await roleManager.RoleExistsAsync("SuperAdmin"))
//     {
//         await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
//     }

//     // Create Super Admin
//     string email = "abanoub.edward97101@gmail.com";
//     string password = "Admin@123";
//     // 🔥 Add these fields to avoid NULL constraint violation


//     var admin = await userManager.FindByEmailAsync(email);
//     if (admin == null)
//     {
//         admin = new IdentityUser
//         {
//             UserName = email,
//             Email = email,
//             EmailConfirmed = true,
//         };
//         await userManager.CreateAsync(admin, password);
//         await userManager.AddToRoleAsync(admin, "SuperAdmin");
//     }

// }

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            ;
    });
});
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGenerallSettingsService, GenerallSettingsService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISalaryReportService, SalaryReportService>();
builder.Services.AddScoped<IOfficialHolidaysService, OfficialHolidaysService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<HrContext>();
//builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<JWTService>();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero; // Validate on every request
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddAutoMapper(Action<IMapperConfigurationExpression> configure);
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<EmployeeProfile>();// add as many profiles as you have
    cfg.AddProfile<DepartmentProfile>();
    cfg.AddProfile<GSProfile>();
    cfg.AddProfile<AttendanceProfile>();

});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddDbContext<HrContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWT>();
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    Console.WriteLine("VAL KEY: " + jwtSettings?.key);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.key!))

    };
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc
    (
        "v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "HR Api",
            Description = "Human resource Management System",
            Contact = new OpenApiContact
            {
                Name = "Abanoub Edward",
                Email = "abanoub.edward97101@gmail.com"

            }
        }

    );
    //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Name = "Authorization",
    //    Type = SecuritySchemeType.ApiKey,
    //    Scheme = "Bearer",
    //    BearerFormat = "JWT",
    //    In = ParameterLocation.Header,
    //    Description = "Enter JWT key here :",

    
    //});
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //         new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            },
    //            Name = "Bearer",
    //            In = ParameterLocation.Header
    //        },
    //        new List<string>()
    //    }
    //});
});
var app = builder.Build();
//await SeedSuperAdminAsync(app);
//app.MapIdentityApi<IdentityUser>();
// Configure the HTTP request pipeline.
using var scope = app.Services.CreateScope();
var serices = scope.ServiceProvider;
var logger = serices.GetRequiredService<ILogger<Program>>();
try
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DefaultRoles.seedAsync(roleManager);
    await DefaultUsers.seedBasicUserAsync(userManager);
    await DefaultUsers.seedSuperAdminAsync(userManager, roleManager);

    logger.LogInformation("Seeding default roles and users completed successfully.");
    logger.LogInformation("App is running...");
}
catch (Exception ex)
{

    logger.LogError(ex, "An error occurred while seeding data.");
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR API v1"); });
    //app.UseSwaggerUI();
}
app.MapHub<ChatHub>("/chat");
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


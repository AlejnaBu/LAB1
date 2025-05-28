using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Lab.Data;
using Lab.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Konfigurimi i `Identity`
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Konfigurimi i `JWT Authentication`
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Konfigurimi i `CORS`
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Konfigurimi i `Swagger`
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard authorization header using Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

//  Sigurohu që rolet dhe admini ekzistojnë
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await EnsureRolesAndAdminAsync(roleManager, userManager);
}

//  Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Funksioni për të krijuar rolet dhe administratorin default
static async Task EnsureRolesAndAdminAsync(RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager)
{
    string[] roleNames = { "admin", "doktor", "patient" };

  
    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            Console.WriteLine($"Roli '{roleName}' u krijua me sukses.");
        }
    }

    var adminEmail = "admin1@mail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "Admin1",
            Email = adminEmail,
            UserRole = "admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Admin1@");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "admin"); 
            Console.WriteLine("Admini u krijua me sukses dhe u shtua në rolin 'admin'!");
        }
    }
    else
    {
        
        var isInRole = await userManager.IsInRoleAsync(adminUser, "admin");
        if (!isInRole)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
            Console.WriteLine("Admini ekzistues u përditësua me rolin 'admin'.");
        }

        
        if (string.IsNullOrEmpty(adminUser.UserRole) || adminUser.UserRole != "admin")
        {
            adminUser.UserRole = "admin";
            await userManager.UpdateAsync(adminUser);
            Console.WriteLine("UserRole në databazë u përditësua për adminin.");
        }
    }
}

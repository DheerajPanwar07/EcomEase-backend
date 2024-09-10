using System.Text;
using EcomEaseContextDb.Context;
using EcomEaseModel.Model;
using EcomEaseRepo.Interface;
using EcomEaseRepo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configure Mail Settings
builder.Services.Configure<MailModel>(builder.Configuration.GetSection("MailSettings"));

// Add services to the container.
builder.Services.AddTransient<IEcomEase, SEcomEase>();
//cors policy
builder.Services.AddCors(option =>
{
	option.AddPolicy("AllowAllOrigins",
		builder =>
		{
			builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
		});
});

// Add DbContext to the container.
builder.Services.AddDbContext<EcomEaseContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

// Add Controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
	 options.TokenValidationParameters = new TokenValidationParameters
	 {
		 ValidateIssuer = true,
		 ValidateAudience = true,
		 ValidateLifetime = true,
		 ValidateIssuerSigningKey = true,
		 ValidIssuer = jwtIssuer,
		 ValidAudience = jwtIssuer,
		 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
	 };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();  // Ensure this is present to map your controllers
app.UseCors("AllowAllOrigins");
app.Run();

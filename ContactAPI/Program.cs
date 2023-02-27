using ContactAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddDbContext<ContactAPIDbContext>(options => options.UseInMemoryDatabase("ContactsDb"));

builder.Services.AddDbContext<ContactAPIDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("ContactsApiConnectionString")));

var key = "This is my Key This is my";

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key)),
        ValidateAudience = false,
        ValidateIssuer = false
    };
}
);

builder.Services.AddSingleton<ContactAPI.IJwtAuthenticationManager>(new ContactAPI.JwtAuthenticationManager(key));



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

app.MapControllers();

app.Run();

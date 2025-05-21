using Microsoft.EntityFrameworkCore;
using Millonario.Infrastructure;
using Millonario.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System; // Agregado para 'new Version()'

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext para MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 35)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

// Configuración de Autenticación JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(); // Asegúrate de que AddAuthorization esté aquí

// ***************************************************************
// AÑADE ESTA SECCIÓN PARA LA CONFIGURACIÓN DE CORS
// ***************************************************************
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173") // ¡VERIFICA QUE ESTA ES LA URL DE TU FRONTEND!
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});
// ***************************************************************

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ***************************************************************
// AÑADE ESTA LÍNEA PARA USAR EL MIDDLEWARE DE CORS
// Debe ir ANTES de UseAuthentication y UseAuthorization
// ***************************************************************
app.UseCors("AllowSpecificOrigin"); // Usa el nombre de la política que definiste arriba
// ***************************************************************

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
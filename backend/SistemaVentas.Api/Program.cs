using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.Errors;
using SistemaVentas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VentasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VentasDB")));

builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddControllers();

// Errores en formato ProblemDetails (RFC 9457) para toda la API.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Los orígenes solo se definen en appsettings.Development.json; fuera de desarrollo la lista queda vacía.
var origenesPermitidos = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(origenesPermitidos)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("Location")));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();

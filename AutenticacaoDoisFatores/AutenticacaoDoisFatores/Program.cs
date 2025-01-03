using AutenticacaoDoisFatores;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Servico.Mapeadores;
using Mensageiro;
using Mensageiro.WebApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CrudContexto>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AutenticacaoDoisFatoresCrudConnection"))
);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAutoMapper(typeof(MapeadorDeCliente));
builder.Services.AddMensageiro();

builder.Services.AddRepositorios();
builder.Services.AddDominios();
builder.Services.AddCasosDeUso();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

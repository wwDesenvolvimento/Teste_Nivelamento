using MediatR;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Services;
using Questao5.Domain.Interfaces.Repository;
using Questao5.Domain.Interfaces.Services;
using Questao5.Infrastructure.Repository;
using Questao5.Infrastructure.Sqlite;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Data;
using FluentAssertions.Common;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(typeof(ConsultaSaldoQuery).Assembly);
builder.Services.AddMediatR(typeof(MovimentarContaHandler).Assembly);


// Register the IDbConnection (SQLite in this case)
builder.Services.AddScoped<IDbConnection>(sp =>
{
    // Configura a string de conexão
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SqliteConnection(connectionString);
});


// Register the handler and repository as Scoped
builder.Services.AddScoped<IRequestHandler<ConsultaSaldoQuery, ConsultaSaldoResult>, ConsultaSaldoQueryHandler>();
builder.Services.AddScoped<IRequestHandler<MovimentacaoRequest, MovimentacaoResponse>, MovimentarContaHandler>();

builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IContaCorrenteService, ContaCorrenteService>();

// sqlite
builder.Services.AddSingleton(new DatabaseConfig 
{ 
    Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") 
});
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html



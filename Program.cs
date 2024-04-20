using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using FluentValidation.AspNetCore;

using REST_API_ANIMALS_2;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterValidators();
builder.Services.AddControllers();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateAnimalRequestValidator>();
// builder.Services.AddScoped<IAnimalService, AnimalService>();
// builder.Services.AddScoped<IDbRepository, SqlDbRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterAnimalsEndpoints();
app.RegisterAnimalsDapperEndpoints();
app.MapControllers();
app.Run();

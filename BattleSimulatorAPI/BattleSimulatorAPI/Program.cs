using BattleSimulatorAPI.DataLayer.Models.Repositories;
using BattleSimulatorAPI.Repositories;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BattleSimDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BattleSimDbConnection")));

// Generic Repository Pattern
builder.Services.AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));

// Specific Repositories
builder.Services.AddScoped<IFighterRepository, FighterRepository>();
builder.Services.AddScoped<IAttackRepository, AttackRepository>();
builder.Services.AddScoped<IFighterTypeRepository, FighterTypeRepository>();
builder.Services.AddScoped<IElementTypeRepository, ElementTypeRepository>();
builder.Services.AddScoped<IFighterAttackRepository, FighterAttackRepository>();
builder.Services.AddScoped<IBreezeRepository, BreezeRepository>();

// CORS Configuration - Allow frontend (Angular) to communicate with API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

//Ensure database migrations are applied automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BattleSimDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Global exception handling middleware for production
    app.UseExceptionHandler("/error");
}

// Enable CORS
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

// SQLite connection string
var connectionString = "Data Source=Pizzas.db";

// Configure EF Core to use SQLite
builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// CRUD endpoints

// Get all pizzas
app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync());

// Get a pizza by id
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) =>
    await db.Pizzas.FindAsync(id) is Pizza pizza ? Results.Ok(pizza) : Results.NotFound());

// Create a new pizza
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    db.Pizzas.Add(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

// Update an existing pizza
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza update, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    pizza.Name = update.Name;
    pizza.Description = update.Description;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Delete a pizza
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();

    return Results.Ok();
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaDb>();
    db.Database.EnsureCreated();
}

app.Run();

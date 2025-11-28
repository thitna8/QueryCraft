using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

class PizzaDb : DbContext
{
    public PizzaDb(DbContextOptions options) : base(options) { }

    public DbSet<Pizza> Pizzas => Set<Pizza>();
}

using System.Reflection;
using Calculator.WpfApp.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace Calculator.WpfApp.Models;

internal class AppDbContext : DbContext
{
	public DbSet<Result> Results { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
		=> optionsBuilder.UseSqlite("Filename=Database/Calculator.db");

	//string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Calculator", "Calculator.db");
	//optionsBuilder.UseSqlite("Data Source=Database/Calculator.db;");

	protected override void OnModelCreating(ModelBuilder modelBuilder) 
		=> modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
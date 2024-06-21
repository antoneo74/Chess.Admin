using ChessDB.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChessDB
{
    public class ChessDbContext:DbContext
    {
        public DbSet<Fen> Fens { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;

            assemblyLocation = assemblyLocation.Remove(assemblyLocation.LastIndexOf(@"\"));

            var path = Path.Combine(assemblyLocation, "Statistic.db");

            optionsBuilder.UseSqlite($"Filename={path}");
        }
    }
}

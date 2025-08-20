using Claymore.Src.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Persistence;

public class DataContext : DbContext
{
    public DataContext() { }
    /*public DataContext(DbContextOptions<DataContext> options) : base(options) { }*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /*optionsBuilder.UseSqlite($"Data Source=..\\db\\Claymore.db");*/
        optionsBuilder.UseSqlite("Data Source=C:\\Users\\aboh.israel\\OneDrive\\Documents\\Codes\\Claymore\\db\\Claymore.db");
    }
    public DbSet<TaskResult> Tasks { get; set; }
}
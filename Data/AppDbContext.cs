using ApiTarefas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiTarefas.Data;

public class AppDbContext : DbContext
{
    public DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("DataSource=banco.db;Cache=Shared");
}

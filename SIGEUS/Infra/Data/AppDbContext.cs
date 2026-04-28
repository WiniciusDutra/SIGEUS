using Microsoft.EntityFrameworkCore;
using SIGEUS.Domain.Entities;
namespace SIGEUS.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Documento> Documento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        });
        
        modelBuilder.Entity<Documento>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Nome).IsRequired().HasMaxLength(50);
            builder.Property(d => d.Extensao).IsRequired().HasMaxLength(4);
            
            builder.HasOne(d => d.Usuario)
                .WithMany(u => u.Documentos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
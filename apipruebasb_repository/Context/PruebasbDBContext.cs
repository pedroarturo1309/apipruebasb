using apipruebasb_entities.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace apipruebasb_repository;
public class PruebasbDBContext : DbContext
{
    public PruebasbDBContext(DbContextOptions<PruebasbDBContext> contextOptions)
                       : base(contextOptions)
    {
    }

    public DbSet<Usuarios> Usuarios { get; set; }

}

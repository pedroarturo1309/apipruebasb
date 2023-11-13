using Microsoft.EntityFrameworkCore;

namespace apipruebasb_repository;
public class PruebasbDBContext : DbContext
{
    public PruebasbDBContext(DbContextOptions<PruebasbDBContext> contextOptions)
                       : base(contextOptions)
    {
    }

}

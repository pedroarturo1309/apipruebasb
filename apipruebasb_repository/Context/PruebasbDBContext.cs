﻿using Microsoft.EntityFrameworkCore;

namespace apipruebasb_repository;
public class PruebasbDBContext : DbContext
{
    public PruebasbDBContext(DbContextOptions<PruebasbDBContext> contextOptions)
                       : base(contextOptions)
    {
    }

    public DbSet<apipruebasb_entities.Usuarios.Usuarios> Usuarios { get; set; }

}

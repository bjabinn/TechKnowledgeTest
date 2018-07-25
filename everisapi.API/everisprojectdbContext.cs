using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace everisapi.API
{
    public partial class everisprojectdbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               //optionsBuilder.UseMySql("server=localhost; port=3306; database=everisprojectdb; user=root; password=");
               optionsBuilder.UseMySql("server=localhost; port=3306; database=everisprojectdb; user=root; password=");
           }
         }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
       {}
    }
}

using iGeoComAPI.Models;
using iGeoComAPI.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<IGeoComModel> IGeoComModels { get; set;}
        public DbSet<IGeoComGrabModel> IGeoComGrabModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IGeoComModel>().HasKey(b => b.GeoNameId).HasName("PK_IGeoComModels");
            modelBuilder.Entity<IGeoComGrabModel>().HasKey(b => b.GrabId).HasName("PK_IGeoComGrabModels");
        }

    }
}

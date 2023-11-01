using System.Data.Common;
using System.Data.Entity;

namespace Akaha_Gesture.Stats {
    public class AkahaGestureDbContext : DbContext {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<SessionImage> SessionImages { get; set; }

        // create in-memory sqlite database - required for generating migration
        public AkahaGestureDbContext() : base("Data Source=:memory:;") {}

        public AkahaGestureDbContext(DbConnection connection) : base(connection, false) {}
        public AkahaGestureDbContext(string connectionString) : base(connectionString) {}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>().ToTable("sessions")
                .HasMany(s => s.sessionImages)
                .WithRequired(si => si.session);
            modelBuilder.Entity<Image>().ToTable("images");
            modelBuilder.Entity<SessionImage>().ToTable("session_images");

            modelBuilder.Entity<SessionImage>()
                .HasKey(si => new { si.sessionId, si.imageId });

            modelBuilder.Entity<SessionImage>()
                .HasRequired(si => si.session)
                .WithMany(s => s.sessionImages)
                .HasForeignKey(si => si.sessionId);

            modelBuilder.Entity<SessionImage>()
                .HasRequired(si => si.image)
                .WithMany()
                .HasForeignKey(si => si.imageId);
        }
    }
}
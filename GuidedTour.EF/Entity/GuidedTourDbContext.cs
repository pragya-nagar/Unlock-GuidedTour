using Microsoft.EntityFrameworkCore;

namespace GuidedTour.EF.Entity
{
    public partial class GuidedTourDbContext : DataContext
    {
        public GuidedTourDbContext()
        {
        }

        public GuidedTourDbContext(DbContextOptions<GuidedTourDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<GuidedTour> GuidedTour { get; set; }
        public virtual DbSet<OnBoardingScreen> OnBoardingScreen { get; set; }
        public virtual DbSet<OnBoardingControl> OnBoardingControl { get; set; }
        public virtual DbSet<GuidedTourControl> GuidedTourControl { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OnBoardingScreen>(entity =>
            {
                entity.HasKey(e => e.ScreenId)
                    .HasName("PK__OnBoardi__0AB60FA5EE2E68D8");
            });

            modelBuilder.Entity<OnBoardingControl>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK__OnBoardi__3214EC07E72CB5DE");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

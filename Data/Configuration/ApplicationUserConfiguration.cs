using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApi.Entities;

namespace MoviesApi.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x=>x.UserName).IsRequired().HasMaxLength(50);
            builder.Property(x=>x.LastName).IsRequired().HasMaxLength(50);
            builder.HasMany(u => u.FavoriteMovies)
                .WithMany()
                .UsingEntity<UserMovie>(
                    l => l.HasOne(u => u.Movie).WithMany().HasForeignKey(u => u.MovieId),
                    r => r.HasOne(m => m.User).WithMany().HasForeignKey(M => M.UserId)
                );
        }
    }
}

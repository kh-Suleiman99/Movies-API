
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApi.Entities;

namespace MoviesApi.Data.Configuration
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.StoryLine).HasMaxLength(250);
            builder.Property(x => x.Author).HasMaxLength(100);
            builder.HasOne(m => m.Genre);
        }
    }
}

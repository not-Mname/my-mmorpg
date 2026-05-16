using Microsoft.EntityFrameworkCore;
using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GameServer.Data.Config
{
    public class TGuildConfiguration : IEntityTypeConfiguration<TGuild>
    {
        public void Configure(EntityTypeBuilder<TGuild> builder)
        {
            builder.ToTable("Guilds");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(g => g.Name).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(g => g.LeaderID).HasColumnType("int").IsRequired();
            builder.Property(g => g.LeaderName).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(g => g.Notice).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(g => g.CreateTime).HasColumnType("datetime").IsRequired();

        }
    }
}

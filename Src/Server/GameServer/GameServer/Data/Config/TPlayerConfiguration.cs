using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameServer.Models.Data;
namespace GameServer.Data.Config
{
    public class TPlayerConfiguration : IEntityTypeConfiguration<TPlayer>
    {
        public void Configure(EntityTypeBuilder<TPlayer> builder)
        {
            builder.ToTable("Players");

            builder.HasKey(p => p.ID);
            builder.Property(p => p.ID).HasColumnType("int").ValueGeneratedOnAdd();
        }
    }
}

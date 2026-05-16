using Microsoft.EntityFrameworkCore;
using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GameServer.Data.Config
{
    public class TGuildApplyConfiguration : IEntityTypeConfiguration<TGuildApply>
    {
        public void Configure(EntityTypeBuilder<TGuildApply> builder)
        {
            builder.ToTable("GuildApplies");

            builder.HasKey(ga => ga.Id);
            builder.Property(ga => ga.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(ga => ga.CharacterId).HasColumnType("int").IsRequired();
            builder.Property(ga => ga.Name).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(ga => ga.Level).HasColumnType("int").IsRequired();
            builder.Property(ga => ga.Result).HasColumnType("int").IsRequired();
            builder.Property(ga => ga.ApplyTime).HasColumnType("datetime").IsRequired();
            builder.Property(ga => ga.GuildId).HasColumnType("int").IsRequired();
            builder.Property(ga => ga.TGuildId).HasColumnType("int").IsRequired();
            builder.Property(ga => ga.Class).HasColumnType("int").IsRequired();

            builder.HasOne(ga => ga.TGuild)
                .WithMany(g => g.Applies)
                .HasForeignKey(ga => ga.TGuildId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

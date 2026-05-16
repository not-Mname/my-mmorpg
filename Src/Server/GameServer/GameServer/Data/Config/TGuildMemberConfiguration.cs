using Microsoft.EntityFrameworkCore;
using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GameServer.Data.Config
{
    public class TGuildMemberConfiguration : IEntityTypeConfiguration<TGuildMember>
    {
        public void Configure(EntityTypeBuilder<TGuildMember> builder)
        {
            builder.ToTable("GuildMembers");

            builder.HasKey(gm => gm.Id);
            builder.Property(gm => gm.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(gm => gm.CharacterId).HasColumnType("int").IsRequired();
            builder.Property(gm => gm.Name).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(gm => gm.Class).HasColumnType("int").IsRequired();
            builder.Property(gm => gm.Level).HasColumnType("int").IsRequired();
            builder.Property(gm => gm.JoinTime).HasColumnType("datetime").IsRequired();
            builder.Property(gm => gm.LastTime).HasColumnType("datetime").IsRequired();
            builder.Property(gm => gm.GuildId).HasColumnType("int").IsRequired();
            builder.Property(gm => gm.TGuildId).HasColumnType("int").IsRequired();
            builder.Property(gm => gm.Title).HasColumnType("int").IsRequired();

            builder.HasOne<TGuild>()
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.TGuildId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

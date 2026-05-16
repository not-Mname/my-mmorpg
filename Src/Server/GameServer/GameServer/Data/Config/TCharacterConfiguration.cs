
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameServer.Models.Data;
namespace GameServer.Data.Config
{
    public class TCharacterConfiguration : IEntityTypeConfiguration<TCharacter>
    {
        public void Configure(EntityTypeBuilder<TCharacter> builder)
        {
            builder.ToTable("Characters");

            builder.HasKey(c => c.ID);
            builder.Property(c => c.ID).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.TID).HasColumnType("int").IsRequired();
            builder.Property(c => c.Name).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(c => c.Class).HasColumnType("int").IsRequired();
            builder.Property(c => c.MapID).HasColumnType("int").IsRequired();
            builder.Property(c => c.MapPosX).HasColumnType("int").IsRequired();
            builder.Property(c => c.MapPosY).HasColumnType("int").IsRequired();
            builder.Property(c => c.MapPosZ).HasColumnType("int").IsRequired();
            builder.Property(c => c.Gold).HasColumnType("bigint").IsRequired();
            builder.Property(c => c.Equips).HasColumnType("binary(28)").IsRequired();
            builder.Property(c => c.Level).HasColumnType("int").IsRequired();
            builder.Property(c => c.EXP).HasColumnType("bigint").IsRequired();
            builder.Property(c => c.GuildId).HasColumnType("int").IsRequired();
            builder.Property(c => c.HP).HasColumnType("int").IsRequired();
            builder.Property(c => c.MP).HasColumnType("int").IsRequired();
            builder.Property(c => c.Player_ID).HasColumnType("int").IsRequired();

            builder.HasOne(c => c.Player)
                .WithMany(p => p.Characters)
                .HasForeignKey(c => c.Player_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Items)
                .WithOne(ci => ci.Owner)
                .HasForeignKey(ci => ci.TCharacterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Bag)
                .WithOne(cb => cb.Owner)
                .HasForeignKey<TCharacterBag>(cb => cb.Owner_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Quests)
                .WithOne(cq => cq.Owner)
                .HasForeignKey(cq => cq.TCharacterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Friends)
                .WithOne(cf => cf.Owner)
                .HasForeignKey(cf => cf.CharacterID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

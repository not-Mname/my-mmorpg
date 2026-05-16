using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameServer.Data.Config
{
    public class TCharacterBagConfiguration : IEntityTypeConfiguration<TCharacterBag>
    {
        public void Configure(EntityTypeBuilder<TCharacterBag> builder)
        {
            builder.ToTable("CharacterBags");

            builder.HasKey(cb => cb.Id);
            builder.Property(cb => cb.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(cb => cb.Items).HasColumnType("varbinary(max)").IsRequired();
            builder.Property(cb => cb.Unlocked).HasColumnType("int").IsRequired();
            builder.Property(cb => cb.Owner_ID).HasColumnType("int").IsRequired();

            builder.HasOne(cb => cb.Owner)
                .WithOne(c => c.Bag)
                .HasForeignKey<TCharacterBag>(cb => cb.Owner_ID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

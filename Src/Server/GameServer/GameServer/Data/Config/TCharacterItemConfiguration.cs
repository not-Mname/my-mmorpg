using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using GameServer.Models.Data;
namespace GameServer.Data.Config
{
    public class TCharacterItemConfiguration : IEntityTypeConfiguration<TCharacterItem>
    {
        public void Configure(EntityTypeBuilder<TCharacterItem> builder)
        {
            builder.ToTable("CharacterItems");

            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(ci => ci.ItemId).HasColumnType("int").IsRequired();
            builder.Property(ci => ci.ItemCount).HasColumnType("int").IsRequired();
            builder.Property(ci => ci.TCharacterID).HasColumnType("int").IsRequired();

            builder.HasOne(ci => ci.Owner)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.TCharacterID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

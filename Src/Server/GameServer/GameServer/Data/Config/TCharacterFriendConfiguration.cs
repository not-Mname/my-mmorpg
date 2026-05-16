using Microsoft.EntityFrameworkCore;
using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GameServer.Data.Config
{
    public class TCharacterFriendConfiguration : IEntityTypeConfiguration<TCharacterFriend>
    {
        public void Configure(EntityTypeBuilder<TCharacterFriend> builder)
        {
            builder.ToTable("CharacterFriends");

            builder.HasKey(cf => cf.Id);
            builder.Property(cf => cf.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(cf => cf.CharacterID).HasColumnType("int").IsRequired();
            builder.Property(cf => cf.FriendName).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(cf => cf.FriendID).HasColumnType("int").IsRequired();
            builder.Property(cf => cf.Level).HasColumnType("int").IsRequired();
            builder.Property(cf => cf.Class).HasColumnType("int").IsRequired();

            builder.HasOne(cf => cf.Owner)
                .WithMany(c => c.Friends)
                .HasForeignKey(cf => cf.CharacterID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

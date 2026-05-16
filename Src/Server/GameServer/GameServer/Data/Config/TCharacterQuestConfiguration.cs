using Microsoft.EntityFrameworkCore;
using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GameServer.Data.Config
{
    public class TCharacterQuestConfiguration : IEntityTypeConfiguration<TCharacterQuest>
    {
        public void Configure(EntityTypeBuilder<TCharacterQuest> builder)
        {
            builder.ToTable("CharacterQuests");

            builder.HasKey(cq => cq.Id);
            builder.Property(cq => cq.Id).HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(cq => cq.TCharacterID).HasColumnType("int").IsRequired();
            builder.Property(cq => cq.QuestID).HasColumnType("int").IsRequired();
            builder.Property(cq => cq.Target1).HasColumnType("int").IsRequired();
            builder.Property(cq => cq.Target2).HasColumnType("int").IsRequired();
            builder.Property(cq => cq.Target3).HasColumnType("int").IsRequired();
            builder.Property(cq => cq.Status).HasColumnType("int").IsRequired();

            builder.HasOne(cq => cq.Owner)
                .WithMany(c => c.Quests)
                .HasForeignKey(cq => cq.TCharacterID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameServer.Models.Data;
namespace GameServer.Data.Config
{
    public class TUserConfiguration : IEntityTypeConfiguration<TUser>
    {
        public void Configure(EntityTypeBuilder<TUser> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.ID);
            builder.Property(u => u.ID).HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(u => u.Username).HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(u => u.Password).HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(u => u.RegisterDate).HasColumnType("datetime");
            builder.Property(u => u.Player_ID).HasColumnType("int").IsRequired();

            builder.HasOne(u => u.Player)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.Player_ID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

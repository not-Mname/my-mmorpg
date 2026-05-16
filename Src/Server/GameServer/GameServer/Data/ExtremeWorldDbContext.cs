using GameServer.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Data
{
    public class ExtremeWorldDbContext : DbContext
    {
        public DbSet<TUser> Users { get; set; }
        public DbSet<TPlayer> Players { get; set; }
        public DbSet<TCharacter> Characters { get; set; }
        public DbSet<TCharacterItem> CharacterItems { get; set; }
        public DbSet<TCharacterBag> CharacterBags { get; set; }
        public DbSet<TCharacterQuest> CharacterQuests { get; set; }
        public DbSet<TCharacterFriend> CharacterFriends { get; set; }
        public DbSet<TGuild> Guilds { get; set; }
        public DbSet<TGuildMember> GuildMembers { get; set; }
        public DbSet<TGuildApply> GuildApplies { get; set; }

        public ExtremeWorldDbContext(DbContextOptions<ExtremeWorldDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 应用所有配置文件
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExtremeWorldDbContext).Assembly);
        }
    }
}
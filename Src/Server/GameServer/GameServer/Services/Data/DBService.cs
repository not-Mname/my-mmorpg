using Common;
using GameServer.Data;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Services.Data
{
    class DBService : Singleton<DBService>, System.IDisposable
    {
        string connectionString;
        ThreadLocal<ExtremeWorldDbContext> _context;

        public ExtremeWorldDbContext Entities => _context.Value;

        public void Init(string connectionString)
        {
            Log.Info("DBService init...");
            this.connectionString = connectionString;
            _context = new ThreadLocal<ExtremeWorldDbContext>(() =>
            {
                var options = new DbContextOptionsBuilder<ExtremeWorldDbContext>()
                    .UseSqlServer(this.connectionString)
                    .Options;
                return new ExtremeWorldDbContext(options);
            });
            bool exist = Entities.Database.CanConnect();
            Log.WarningFormat("DB connection is {0}", exist);
        }

        /// <summary>
        /// 持久化当前线程所有被追踪实体的改动
        /// </summary>
        public void Save(bool async = false)
        {
            if (async)
                Entities.SaveChangesAsync();
            else
                Entities.SaveChanges();
        }

        /// <summary>
        /// 开启一次数据库操作，Dispose 时自动 Save
        /// </summary>
        public DBOperation BeginScope()
        {
            return new DBOperation(this);
        }

        public void Dispose()
        {
            foreach (var context in _context.Values)
            {
                context.SaveChanges();
            }
            _context?.Dispose();
        }
    }

    class DBOperation : System.IDisposable
    {
        DBService _db;
        bool _disposed;

        public DBOperation(DBService db)
        {
            _db = db;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _db.Save();
                _disposed = true;
            }
        }
    }
}

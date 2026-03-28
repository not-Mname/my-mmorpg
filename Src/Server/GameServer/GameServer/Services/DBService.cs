using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            Log.Info("DBService init...");
            entities = new ExtremeWorldEntities();
            bool exist = false;
            if (entities != null)
            {
                exist = entities.Database.Exists();
            }
            Log.WarningFormat("DB connection is {0}", exist);
        }

        public void Save(bool async = false)
        {
            if (async)
                entities.SaveChangesAsync();
            else
                entities.SaveChanges();
        }
    }
}

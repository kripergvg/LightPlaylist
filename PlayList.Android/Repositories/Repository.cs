using System;
using System.IO;
using System.Linq;
using SQLite;

namespace PlayList.Android.Repositories
{
    public class Repository<T> where T : new()
    {
        private readonly string _dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");

        public Repository()
        {
            Db = new SQLiteConnection(_dbPath);
            Db.CreateTable<T>();
        }

        protected SQLiteConnection Db { get; }

        public int Add(T model)
        {
           return Db.Insert(model);
        }

        public T Update(object key, T model)
        {
            var existed = GetById(key);
            if (existed == null)
            {
                Db.Insert(model);
            }
            else
            {
                Db.Update(model);
            }

            return model;
        }

        public T GetById(object id)
        {
            try
            {
                return Db.Get<T>(id);
            }
            catch (InvalidOperationException)
            {
                return default(T);
            }          
        }

        public T[] Get()
        {
            return Db.Table<T>().ToArray();
        }

        public void Delete(object key)
        {
            Db.Delete<T>(key);
        }
    }
}
using System;
using PlayList.Android.DatabaseModels;

namespace PlayList.Android.Repositories
{
    public class SyncRepository: Repository<Audio>
    {
        public DateTime? GetLastDate()
        {
            return Db.Table<Audio>().OrderByDescending(a => a.AddedDate).FirstOrDefault()?.AddedDate;
        }
    }
}
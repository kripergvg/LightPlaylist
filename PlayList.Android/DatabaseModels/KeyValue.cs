using SQLite;

namespace PlayList.Android.DatabaseModels
{
    public class KeyValue
    {
        [PrimaryKey]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
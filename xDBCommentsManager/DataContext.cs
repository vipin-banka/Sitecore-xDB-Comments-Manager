#region

using MongoDB.Driver;
using Sitecore.Configuration;

#endregion

namespace xDBCommentsManager
{
    internal class DataContext
    {
        // Get connection string from sitecore config
        private static string CONNECTION_STRING_NAME = Settings.GetSetting("CONNECTION_STRING_NAME");

        // Get database name from sitecore config
        private static string DATABASE_NAME = Settings.GetSetting("DATABASE_NAME");

        // Get collection name from sitecore config
        private string COMMENTS_COLLECTION_NAME = Settings.GetSetting("COMMENTS_COLLECTION_NAME");

        private static MongoServer _client;
        private static MongoDatabase _database;

        public DataContext()
        {

            // Get mongoclient            
             _client = new MongoClient(CONNECTION_STRING_NAME).GetServer();
                    
            // Get database from mongo
            _database = _client.GetDatabase(DATABASE_NAME);
        }

        public MongoServer Client
        {
            get { return _client; }
        }

        public MongoCollection<Comment> Comments
        {
            // Retrieve collection from mongo
            get { return _database.GetCollection<Comment>(COMMENTS_COLLECTION_NAME); }
        }
    }
}
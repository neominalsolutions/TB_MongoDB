using MongoDB.Bson;
using MongoDbDriverSampleApp.Documents.Posts;

namespace MongoDbDriverSampleApp.Documents
{
    public class Post
    {
        public ObjectId Id { get; set; } // Dokumanlarda Id vardır, Mebeded Resource da Id tutmayız

        public string Title { get; set; }

        public string Content { get; set; }

        public List<Comment> Comments { get; set; }

        public List<string> Tags { get; set; }

        public User User { get; set; }



    }
}

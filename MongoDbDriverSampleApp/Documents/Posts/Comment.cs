namespace MongoDbDriverSampleApp.Documents.Posts
{
  public class Comment
  {
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } 
    public User CommentUser { get; set; } // Yorumu yapan kullanıcı

  }
}

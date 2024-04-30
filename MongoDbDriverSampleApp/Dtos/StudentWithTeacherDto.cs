namespace MongoDbDriverSampleApp.Dtos
{
  public class TeacherDto
  {
    public string Id { get; set; }
    public string Name { get; set; }

  }
  public class StudentWithTeacherDto
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string ClassName { get; set; }

    public List<TeacherDto> Teachers { get; set; } = new List<TeacherDto>();


  }
}

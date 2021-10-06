namespace PegasusApi.Library.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProjectPrefix { get; set; }
        public bool IsPinned { get; set; }
        public bool IsActive { get; set; }
    }
}

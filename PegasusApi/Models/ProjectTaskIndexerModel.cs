namespace PegasusApi.Models
{
    public class ProjectTaskIndexerModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int NextIndex { get; set; }
    }
}

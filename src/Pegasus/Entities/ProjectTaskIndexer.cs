using System.ComponentModel.DataAnnotations;

namespace Pegasus.Entities
{
    public class ProjectTaskIndexer
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int NextIndex { get; set; }
    }
}

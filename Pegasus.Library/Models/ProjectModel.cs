using System.ComponentModel;

namespace Pegasus.Library.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        [DisplayName("Title")]
        public string Name { get; set; }
        [DisplayName("Project Prefix")]
        public string ProjectPrefix { get; set; }
        [DisplayName("Pinned")]
        public bool IsPinned { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}

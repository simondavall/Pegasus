using System;
using System.ComponentModel.DataAnnotations;
using Pegasus.Entities.Enumerations;

namespace Pegasus.Entities
{
    public class ProjectTask
    {
        public int Id { get; set; }
        [Display(Name="Ref")]
        public string TaskRef { get; set; }
        [Display(Name = "Title")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Type")]
        public int TaskTypeId { get; set; }
        [Display(Name = "Status")]
        public int TaskStatusId { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Priority")]
        public int TaskPriorityId { get; set; }
        [Display(Name = "Fixed In Release")]
        public string FixedInRelease { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }

        public bool IsClosed => TaskStatusId == (int) TaskStatusEnum.Completed || TaskStatusId == (int) TaskStatusEnum.Obsolete;
    }

}

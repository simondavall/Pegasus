namespace Pegasus.Entities.Profiles
{
    public class OnHoldTaskProfile : ITaskProfile
    {
        public string TaskRefStyle => "";
        public string TaskTextStyle => "";
        public string TaskTimeStyle => "task-time";
        public string TaskIcon => "<i class=\"fa fa-pause-circle-o\" aria-hidden=\"true\" data-toggle=\"tooltip\" title=\"Task On Hold\"></i>";
    }
}

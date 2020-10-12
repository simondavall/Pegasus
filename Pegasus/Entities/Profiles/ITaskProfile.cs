namespace Pegasus.Entities.Profiles
{
    public interface ITaskProfile
    {
        string TaskRefStyle { get; }
        string TaskTextStyle { get; }
        string TaskTimeStyle { get; }
        string TaskIcon { get; }
    }
}

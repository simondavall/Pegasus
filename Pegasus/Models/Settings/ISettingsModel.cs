namespace Pegasus.Models.Settings
{
    public interface ISettingsModel
    {
        bool PaginationDisabled { get; set; }
        int PageSize { get; set; }
    }
}
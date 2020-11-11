namespace Pegasus.Models.Settings
{
    public interface ISettingsModel
    {
        string Title { get; set; }
        int PageSize { get; set; }
        bool ShowPagination { get; set; }
    }
}
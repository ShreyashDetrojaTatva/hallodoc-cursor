using HalloDoc.Services.Helpers;

namespace HalloDoc.Services.Helpers
{
    public interface IWorkContext
    {
        ContextUser? CurrentUser();
    }
} 
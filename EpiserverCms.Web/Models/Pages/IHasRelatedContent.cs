using EPiServer.Core;

namespace EpiserverCms.Web.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}

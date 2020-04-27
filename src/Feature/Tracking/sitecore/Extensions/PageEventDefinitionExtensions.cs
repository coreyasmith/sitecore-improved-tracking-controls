using Sitecore;
using Sitecore.Common;
using Sitecore.Marketing.Definitions.PageEvents;

namespace CoreyAndRick.Feature.Tracking.Extensions
{
    public static class PageEventDefinitionExtensions
    {
        public static bool IsFailure(this IPageEventDefinition pageEvent)
        {
            var pageEventItem = Context.ContentDatabase.GetItem(pageEvent.Id.ToID());
            return MainUtil.GetBool(pageEventItem[SitecoreTemplates.PageEvent.Fields.IsFailure], false);
        }
    }
}

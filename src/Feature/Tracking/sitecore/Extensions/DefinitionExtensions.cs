using Sitecore.Common;
using Sitecore.Data.Managers;
using Sitecore.Marketing.Definitions;

namespace CoreyAndRick.Feature.Tracking.Extensions
{
    public static class DefinitionExtensions
    {
        public static bool IsDeployed(this IDefinition definition)
        {
            var eventItemId = definition.Id.ToID();
            var eventItem = Sitecore.Context.ContentDatabase.GetItem(eventItemId);
            foreach (var language in eventItem.Languages)
            {
                var latestVersionForLanguage = ItemManager.GetItem(eventItemId, language, Sitecore.Data.Version.Latest, eventItem.Database);
                var workflowState = latestVersionForLanguage.State.GetWorkflowState();
                if (workflowState == null) continue;
                if (workflowState.FinalState) return true;
            }
            return false;
        }
    }
}

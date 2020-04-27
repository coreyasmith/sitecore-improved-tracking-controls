using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics.Data;
using Sitecore.Marketing.Definitions.PageEvents;

namespace CoreyAndRick.Feature.Tracking.Extensions
{
    public static class MarketingDefinitionsExtensions
    {
        public static IEnumerable<IPageEventDefinition> Events(
            this IMarketingDefinitions marketingDefinitions)
        {
            return marketingDefinitions.PageEvents
                .Where(pageEvent => pageEvent.IsDeployed() && !pageEvent.IsSystem && !pageEvent.IsFailure());
        }

        public static IEnumerable<IPageEventDefinition> Failures(
            this IMarketingDefinitions marketingDefinitions)
        {
            return marketingDefinitions
                .PageEvents.Where(pageEvent => pageEvent.IsDeployed() && !pageEvent.IsSystem && pageEvent.IsFailure());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using CoreyAndRick.Feature.Tracking.Extensions;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Marketing.Definitions.Goals;
using Sitecore.Marketing.xMgmt.Extensions;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using TreeList = Sitecore.Support.Shell.Applications.ContentEditor.TreeList;

namespace CoreyAndRick.Feature.Tracking.sitecore.shell.Applications.CoreyAndRick
{
    public class TreeListGoals : SheerTrackingFieldPageBase
    {
        protected TreeList TreeList;

        protected override void OnOK(object sender, EventArgs args)
        {
            var packet = new Packet("tracking", Array.Empty<string>());
            AddExistingIgnoreFlag(packet);
            AddExistingProfiles(packet);
            AddExistingCampaigns(packet);
            if (HttpContext.Current == null) return;

            AddExistingPageEvents(packet);

            var treeListValues = ID.ParseArray(TreeList?.GetValue());
            AddSelectedGoals(packet, treeListValues);

            SheerResponse.SetDialogValue(packet.ToString());
            base.OnOK(sender, args);
        }

        protected override void Render(XDocument trackingFieldXml)
        {
            TreeList.Source = GetGoalsFolderPath();
            TreeList.ExcludeItemsForDisplay = string.Join(",", GetHiddenGoalIds());
            TreeList.IncludeTemplatesForSelection = string.Join(",", GetGoalTemplateNames());

            var page = GetPage();
            if (page == null || page.IsPostBack) return;

            var selectedGoals = GetSelectedGoals(trackingFieldXml);
            var treeListValues = string.Join("|", selectedGoals);
            TreeList.SetValue(treeListValues);
        }

        protected virtual string GetGoalsFolderPath()
        {
            var goalsFolder = Context.ContentDatabase.GetItem(WellKnownIdentifiers.MarketingCenterGoalsContainerId);
            return goalsFolder.Paths.FullPath;
        }

        protected virtual IEnumerable<string> GetHiddenGoalIds()
        {
            var hiddenGoalIds = Tracker.MarketingDefinitions.Goals
              .Where(g => g.IsSystem || !g.IsDeployed()).Select(g => g.Id.ToID().ToString());
            return hiddenGoalIds;
        }

        protected virtual IEnumerable<string> GetGoalTemplateNames()
        {
            var goalTemplate = Context.ContentDatabase.GetItem(WellKnownIdentifiers.GoalDefinitionTemplateId);
            var goalTemplateNames = new[]
            {
        goalTemplate.Name
      };
            return goalTemplateNames;
        }
    }
}

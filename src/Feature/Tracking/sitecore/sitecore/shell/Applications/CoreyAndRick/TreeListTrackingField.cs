using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using CoreyAndRick.Feature.Tracking.Extensions;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Globalization;
using Sitecore.Marketing.xMgmt.Extensions;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using CampaignIds = Sitecore.Marketing.Definitions.Campaigns.WellKnownIdentifiers;
using PageEventIds = Sitecore.Marketing.Definitions.PageEvents.WellKnownIdentifiers;
using TreeList = Sitecore.Support.Shell.Applications.ContentEditor.TreeList;

namespace CoreyAndRick.Feature.Tracking.sitecore.shell.Applications.CoreyAndRick
{
    public class TreeListTrackingField : SheerTrackingFieldPageBase
    {
        private const string ContentEditorButtonPath = "Content Editor/Ribbons/Chunks/Analytics - Attributes/Attributes";

        protected TreeList CampaignsList;
        protected TreeList EventsList;
        protected TreeList FailuresList;
        protected Checkbox Ignore;

        protected override void OnOK(object sender, EventArgs args)
        {
            var packet = new Packet("tracking", Array.Empty<string>());
            if (HttpContext.Current == null) return;

            if (Ignore.Checked)
            {
                AddIgnoreFlag(packet);
            }

            AddExistingProfiles(packet);

            var selectedCampaigns = ID.ParseArray(CampaignsList?.GetValue());
            AddSelectedCampaigns(packet, selectedCampaigns);

            AddExistingGoals(packet);

            var selectedFailures = ID.ParseArray(FailuresList?.GetValue()); ;
            AddSelectedPageEvents(packet, selectedFailures);

            var selectedPageEvents = ID.ParseArray(EventsList?.GetValue());
            AddSelectedPageEvents(packet, selectedPageEvents);

            SheerResponse.SetDialogValue(packet.ToString());
            base.OnOK(sender, args);
        }

        protected override void Render(XDocument trackingFieldXml)
        {
            if (trackingFieldXml == null) throw new ArgumentNullException(nameof(trackingFieldXml));

            var page = GetPage();
            var isPostBack = page == null || page.IsPostBack;

            RenderCampaigns(trackingFieldXml, isPostBack);
            RenderEvents(trackingFieldXml, isPostBack);
            RenderFailures(trackingFieldXml, isPostBack);
            RenderIgnore(trackingFieldXml, isPostBack);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            ApplySecurity();
            base.OnLoad(e);

            if (Context.ClientPage.IsEvent) return;
            Ignore.Header = Translate.Text(Ignore.Header);
        }

        protected virtual void ApplySecurity()
        {
            Assert.CanRunApplication(ContentEditorButtonPath);
        }

        protected virtual void RenderCampaigns(
          XDocument trackingFieldXml,
          bool isPostBack)
        {
            CampaignsList.Source = GetMarketingContainerPath(CampaignIds.MarketingCenterCampaignsContainerId);
            CampaignsList.ExcludeItemsForDisplay = string.Join(",", GetHiddenCampaignIds());
            CampaignsList.IncludeTemplatesForSelection = string.Join(",", GetMarketingTemplateNames(CampaignIds.CampaignActivityDefinitionTemplateId));

            if (isPostBack) return;

            var selectedCampaigns = GetSelectedCampaigns(trackingFieldXml);
            var campaignsListValues = string.Join("|", selectedCampaigns);
            CampaignsList.SetValue(campaignsListValues);
        }

        protected virtual IEnumerable<string> GetHiddenCampaignIds()
        {
            var hiddenGoalIds = Tracker.MarketingDefinitions.Campaigns
              .Where(c => !c.IsDeployed()).Select(g => g.Id.ToID().ToString());
            return hiddenGoalIds;
        }

        protected virtual void RenderEvents(
          XDocument trackingFieldXml,
          bool isPostBack)
        {
            EventsList.Source = GetMarketingContainerPath(PageEventIds.PageEventsContainerId);
            EventsList.ExcludeItemsForDisplay = string.Join(",", GetHiddenEventIds());
            EventsList.IncludeTemplatesForSelection = string.Join(",", GetMarketingTemplateNames(PageEventIds.PageEventDefinitionTemplateId));

            if (isPostBack) return;

            var selectedEvents = GetSelectedPageEvents(trackingFieldXml);
            var eventsListValues = string.Join("|", selectedEvents);
            EventsList.SetValue(eventsListValues);
        }

        protected virtual IEnumerable<string> GetHiddenEventIds()
        {
            var hiddenEventIds = Tracker.MarketingDefinitions.PageEvents
              .Where(e => e.IsSystem || e.IsFailure() || !e.IsDeployed()).Select(e => e.Id.ToID().ToString());
            return hiddenEventIds;
        }

        protected virtual void RenderFailures(
          XDocument trackingFieldXml,
          bool isPostBack)
        {
            FailuresList.Source = GetMarketingContainerPath(PageEventIds.PageEventsContainerId);
            FailuresList.ExcludeItemsForDisplay = string.Join(",", GetHiddenFailureIds());
            FailuresList.IncludeTemplatesForSelection = string.Join(",", GetMarketingTemplateNames(PageEventIds.PageEventDefinitionTemplateId));

            if (isPostBack) return;

            var selectedFailures = GetSelectedFailures(trackingFieldXml);
            var failuresListValues = string.Join("|", selectedFailures);
            FailuresList.SetValue(failuresListValues);
        }

        protected virtual IEnumerable<string> GetHiddenFailureIds()
        {
            var hiddenFailureIds = Tracker.MarketingDefinitions.PageEvents
              .Where(e => e.IsSystem || !e.IsFailure() || !e.IsDeployed()).Select(e => e.Id.ToID().ToString());
            return hiddenFailureIds;
        }

        protected virtual void RenderIgnore(
          XDocument trackingFieldXml,
          bool isPostBack)
        {
            if (isPostBack) return;
            Ignore.Checked = trackingFieldXml.Root?.GetAttributeValue("ignore") == "1";
        }

        protected virtual string GetMarketingContainerPath(Guid containerId)
        {
            var container = Context.ContentDatabase.GetItem(containerId);
            return container.Paths.FullPath;
        }

        protected virtual IEnumerable<string> GetMarketingTemplateNames(Guid templateId)
        {
            var template = Context.ContentDatabase.GetItem(templateId);
            var templateNames = new[]
            {
        template.Name
      };
            return templateNames;
        }
    }
}

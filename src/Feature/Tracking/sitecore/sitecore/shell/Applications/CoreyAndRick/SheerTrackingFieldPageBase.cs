using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using CoreyAndRick.Feature.Tracking.Extensions;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Marketing.Definitions;
using Sitecore.Web;
using Sitecore.Web.UI.Pages;
using Sitecore.Xml;

namespace CoreyAndRick.Feature.Tracking.sitecore.shell.Applications.CoreyAndRick
{
    public abstract class SheerTrackingFieldPageBase : DialogForm
    {
        protected virtual string Tracking
        {
            get => StringUtil.GetString(ServerProperties[nameof(Tracking)]);
            set => ServerProperties[nameof(Tracking)] = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected virtual void AddIgnoreFlag(Packet packet)
        {
            packet.SetAttribute("ignore", "1");
        }

        protected virtual void AddSelectedCampaigns(
          Packet packet,
          IEnumerable<ID> selectedCampaignIds)
        {
            AddSelectedEvents(packet, selectedCampaignIds, Tracker.MarketingDefinitions.Campaigns, "campaign", "title");
        }

        protected virtual void AddSelectedGoals(
          Packet packet,
          IEnumerable<ID> selectedGoalIds)
        {
            AddSelectedEvents(packet, selectedGoalIds, Tracker.MarketingDefinitions.Goals, "event", "name");
        }

        protected virtual void AddSelectedPageEvents(
          Packet packet,
          IEnumerable<ID> selectedPageEventIds)
        {
            AddSelectedEvents(packet, selectedPageEventIds, Tracker.MarketingDefinitions.PageEvents, "event", "name");
        }

        protected virtual void AddSelectedEvents(
          Packet packet,
          IEnumerable<ID> selectedEventIds,
          IEnumerable<IDefinition> marketingDefinitions,
          string elementName,
          string nameAttribute)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));
            if (selectedEventIds == null) throw new ArgumentNullException(nameof(selectedEventIds));

            var eventDefinitions = marketingDefinitions.ToList();
            foreach (var selectedEventId in selectedEventIds)
            {
                var selectedPageEvent = eventDefinitions.FirstOrDefault(pageEvent => pageEvent.Id == selectedEventId.Guid);
                if (selectedPageEvent == null) continue;

                packet.AddElement(elementName, string.Empty, "id", selectedPageEvent.Id.ToID().ToString(), nameAttribute, selectedPageEvent.Alias);
            }
        }

        protected virtual Page GetPage()
        {
            return Context.Page?.Page;
        }

        protected virtual void AddExistingCampaigns(Packet packet)
        {
            AddExistingTrackingItems(packet, "/*/campaign");
        }

        protected virtual void AddExistingGoals(Packet packet)
        {
            var deployedGoals = Tracker.MarketingDefinitions.Goals.Where(e => e.IsDeployed());
            AddExistingTrackingItems(packet, "/*/event", deployedGoals);
        }

        protected virtual void AddExistingPageEvents(Packet packet)
        {
            var deployedPageEvents = Tracker.MarketingDefinitions.PageEvents.Where(e => e.IsDeployed());
            AddExistingTrackingItems(packet, "/*/event", deployedPageEvents);
        }

        protected virtual void AddExistingProfiles(Packet packet)
        {
            AddExistingTrackingItems(packet, "/*/profile");
        }

        protected virtual void AddExistingTrackingItems(
          Packet packet,
          string xpath,
          IEnumerable<IDefinition> marketingDefinitions = null)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));
            if (xpath == null) throw new ArgumentNullException(nameof(xpath));

            var xmlNodeList = GetXmlDocument().SelectNodes(xpath);
            if (xmlNodeList == null) return;

            var definitions = marketingDefinitions?.ToList();
            foreach (XmlNode node in xmlNodeList)
            {
                var name = XmlUtil.GetAttribute("name", node);
                if (definitions == null || definitions.Any(definition => definition.Alias == name && definition.IsActive))
                {
                    packet.AddXmlNode(node);
                }
            }
        }

        protected virtual void AddExistingIgnoreFlag(Packet packet)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));

            var documentElement = GetXmlDocument().DocumentElement;
            if (documentElement == null) return;
            if (XmlUtil.GetAttribute("ignore", documentElement) != "1") return;

            packet.SetAttribute("ignore", "1");
        }

        protected override void OnLoad(EventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                Tracking = StringUtil.GetString(UrlHandle.Get()["tracking"], "<tracking />");
            }

            Render(GetDocument());
        }

        protected abstract void Render(XDocument trackingFieldXml);

        protected virtual XDocument GetDocument()
        {
            try
            {
                return XDocument.Parse(Tracking);
            }
            catch (Exception ex)
            {
                Log.Error("Invalid tracking XML", ex, GetType());
                return XDocument.Parse("<tracking />");
            }
        }

        protected virtual XmlDocument GetXmlDocument()
        {
            XmlDocument xmlDocument;
            try
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(Tracking);
            }
            catch (Exception ex)
            {
                Log.Error("Invalid tracking XML", ex, GetType());
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<tracking />");
            }
            return xmlDocument;
        }

        protected virtual IEnumerable<string> GetSelectedCampaigns(XDocument trackingFieldXml)
        {
            return GetSelectedDefinitions(trackingFieldXml, "campaign", Tracker.MarketingDefinitions.Campaigns);
        }

        protected virtual IEnumerable<string> GetSelectedFailures(XDocument trackingFieldXml)
        {
            return GetSelectedDefinitions(trackingFieldXml, "event", Tracker.MarketingDefinitions.Failures());
        }

        protected virtual IEnumerable<string> GetSelectedGoals(XDocument trackingFieldXml)
        {
            return GetSelectedDefinitions(trackingFieldXml, "event", Tracker.MarketingDefinitions.Goals);
        }

        protected virtual IEnumerable<string> GetSelectedPageEvents(XDocument trackingFieldXml)
        {
            return GetSelectedDefinitions(trackingFieldXml, "event", Tracker.MarketingDefinitions.Events());
        }

        protected virtual IEnumerable<string> GetSelectedDefinitions(
          XDocument trackingFieldXml,
          string elementName,
          IEnumerable<IDefinition> definitionCollection)
        {
            var selectedEvents = trackingFieldXml.Descendants(elementName).Select(e => e.GetAttributeValue("id"));
            var selectedDefinitions = ParseIds(selectedEvents)
              .Where(i => definitionCollection.Any(g => g.Id == i.Guid)).Select(i => i.ToString());
            return selectedDefinitions;
        }

        protected virtual IEnumerable<ID> ParseIds(IEnumerable<string> stringIds)
        {
            if (stringIds == null) throw new ArgumentNullException(nameof(stringIds));

            var ids = new List<ID>();
            foreach (var stringId in stringIds)
            {
                if (ID.TryParse(stringId, out var id))
                {
                    ids.Add(id);
                }
            }
            return ids;
        }
    }
}

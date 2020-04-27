using System;
using CoreyAndRick.Feature.Tracking.sitecore.shell.Applications.CoreyAndRick;
using Sitecore;
using Sitecore.Shell.Applications.Analytics.TrackingField;
using Sitecore.Web.UI.Sheer;

namespace CoreyAndRick.Feature.Tracking.Commands
{
    public class OpenTreeListGoals : OpenGoals
    {
        protected override string GetUrl()
        {
            return UIUtil.GetUri($"control:{nameof(TreeListGoals)}");
        }

        protected override void ShowDialog(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            SheerResponse.ShowModalDialog(url, "800px", string.Empty, string.Empty, true);
        }
    }
}

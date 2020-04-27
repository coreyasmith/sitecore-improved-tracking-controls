define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  Sitecore.Commands.OpenAttributes =
  {
    canExecute: function (context) {
      if (context.currentContext.isFallback) {
        return false;
      }

      return context.app.canExecute("ExperienceEditor.OpenTrackingField.CanOpen", context.currentContext);
    },
    execute: function (context) {
      context.currentContext.value = "/sitecore/shell/default.aspx?xmlcontrol=TreeListTrackingField";
      ExperienceEditor.PipelinesUtil.initAndExecutePipeline(context.app.OpenTrackingFieldPipeline, context);
    }
  };
});

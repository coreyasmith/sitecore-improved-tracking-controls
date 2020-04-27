define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  Sitecore.Commands.OpenGoals =
  {
    canExecute: function (context) {
      if (context.currentContext.isFallback) {
        return false;
      }

      return context.app.canExecute("ExperienceEditor.OpenTrackingField.CanOpen", context.currentContext);
    },
    execute: function (context) {
      context.currentContext.value = "/sitecore/shell/default.aspx?xmlcontrol=TreeListGoals";
      ExperienceEditor.PipelinesUtil.initAndExecutePipeline(context.app.OpenTrackingFieldPipeline, context);
    }
  };
});

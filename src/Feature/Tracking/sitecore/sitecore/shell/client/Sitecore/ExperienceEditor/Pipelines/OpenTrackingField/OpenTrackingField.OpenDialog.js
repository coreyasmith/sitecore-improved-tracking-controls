define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  return ExperienceEditor.PipelinesUtil.generateDialogCallProcessor({
    url: function (context) { return context.currentContext.value; },
    features: "dialogHeight: 600px;dialogWidth: 800px;",
    onSuccess: function (context, trackingField) {
      context.currentContext.value = ExperienceEditor.Web.encodeHtml(trackingField);
    }
  });
});

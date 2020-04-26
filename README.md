# ✨ Improved Tracking Controls

This repository contains code for the improved tracking controls that
[Rick Bauer][1] and I demonstrated in our _Crank Up Your Sitecore Authoring
and Marketing Experience_ presentation.

## 🏗️ Setup

### 🐳 Docker

1. Build the Sitecore 9.3 docker images using the steps in the
   [Sitecore Docker images repository][2].
2. Build the solution with the `Docker` build configuration.
3. Publish the projects in the solution with the `Docker` publish profile.
4. On the command line:
   1. `cd C:\[path-to]\sitecore-improved-page-attribute-controls`
   2. `docker-compose up`

### 💽 Locally

1. Install a new instance of [Sitecore 9.3][3].
2. Update the `publishUrl` in [`PublishSettings.Sitecore.targets`][4] to your
   Sitecore installation's web root (e.g., `C:\inetpub\wwwroot\sc93.sc`).
3. Build the solution with the `Debug` build configuration.
4. Publish the projects in the solution with the `Local` publish profile.

[1]: https://twitter.com/Sitecordial
[2]: https://github.com/sitecore/docker-images
[3]: https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/93/Sitecore_Experience_Platform_93_Initial_Release.aspx
[4]: PublishSettings.Sitecore.targets

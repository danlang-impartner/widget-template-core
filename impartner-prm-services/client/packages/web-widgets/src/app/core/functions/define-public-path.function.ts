import { widgetRuntime } from '@impartner/widget-runtime';

import { WIDGETS } from '../widgets';

export function definePublicPath(): string | undefined {
  let publicPath: string | undefined;

  if (widgetRuntime.services.appConfig.resourceUrl) {
    publicPath = widgetRuntime.services.appConfig.resourceUrl;
    publicPath = `${publicPath}/dist/@impartner/web-widgets/`;
  }

  WIDGETS.forEach(widgetMetadata => {
    const publicPathWidget = widgetRuntime.manager.getResourceUrl(widgetMetadata);

    if (publicPathWidget) {
      publicPath = publicPathWidget;
    }
  });

  return publicPath;
}

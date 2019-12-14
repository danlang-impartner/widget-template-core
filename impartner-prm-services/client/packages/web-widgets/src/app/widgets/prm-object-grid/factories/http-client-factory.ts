import { IHttpClient, IWidgetRuntime } from '@impartner/widget-runtime';

import { HttpClientAdapter } from '../adapters';

export function httpClientFactory(
  httpClientAdapter: HttpClientAdapter,
  widgetRuntime: IWidgetRuntime
): IHttpClient {
  let httpClientInstance = widgetRuntime.services.httpClient;

  if (!httpClientInstance) {
    httpClientInstance = httpClientAdapter;
  }

  return httpClientInstance;
}

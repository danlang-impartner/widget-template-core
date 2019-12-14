import { Inject, Injectable } from '@angular/core';
import { IWidgetRuntime } from '@impartner/widget-runtime';

import { CORE_TOKENS } from 'src/app/core/constants';

@Injectable()
export class EndpointLocatorService {
  constructor(@Inject(CORE_TOKENS.WidgetRuntime) private readonly _widgetRuntime: IWidgetRuntime) {}

  public getApiUrl(): string {
    let endpointUrl = '';

    if (!this._widgetRuntime.services.httpClient) {
      endpointUrl = `${window.location.protocol}//${window.location.hostname}:${
        window.location.port
      }/api`;
    }

    return endpointUrl;
  }
}

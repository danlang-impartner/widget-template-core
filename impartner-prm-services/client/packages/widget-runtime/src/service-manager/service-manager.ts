import { Logger } from '@impartner/config-utils';

import { IWindow } from 'src/shared';
import { IAppConfig, IHttpClient, IRoutingStrategy } from './interfaces';

export class ServiceManager {
  private _routingStrategy: IRoutingStrategy | null = null;

  get httpClient(): IHttpClient {
    const PARENT_HTTP_CLIENT = 'PARENT_HTTP_CLIENT';

    if (this._isInIframe()) {
      return (window as IWindow).parent[PARENT_HTTP_CLIENT];
    }

    return (window as IWindow)[PARENT_HTTP_CLIENT];
  }

  public get appConfig(): IAppConfig {
    const APP_CONFIG_NAMESPACE = 'appConfig';
    const defaultAppConfig: IAppConfig = {
      resourceUrl: '/'
    };
    let appConfig = (window as IWindow)[APP_CONFIG_NAMESPACE];

    if (!appConfig) {
      appConfig = defaultAppConfig;
    }

    return appConfig;
  }

  public get router(): IRoutingStrategy | null {
    return this._routingStrategy;
  }

  public registerRoutingStrategy(strategy: IRoutingStrategy): void {
    if (this._routingStrategy) {
      Logger.warn('A widget routing strategy is already defined. Skipping registration.');

      return;
    }

    if (!strategy) {
      throw new Error('registerRoutingStrategy: A strategy must be defined.');
    }

    this._routingStrategy = strategy;
  }

  private _isInIframe(): boolean {
    return window.self !== window.top;
  }
}

import { IWindow } from 'src/shared';
import { ServiceManager } from './service-manager';

describe('service-manager.ts', () => {
  let serviceManager: ServiceManager;

  beforeEach(() => {
    serviceManager = new ServiceManager();
  });

  afterEach(() => {
    // tslint:disable-next-line: no-string-literal
    delete (window as IWindow)['PARENT_HTTP_CLIENT'];
    // tslint:disable-next-line: no-string-literal
    delete (window as IWindow).parent['PARENT_HTTP_CLIENT'];
    // tslint:disable-next-line: no-string-literal
    delete (window as IWindow)['appConfig'];
  });

  describe('get httpClient()', () => {
    it('should return the httpClient defined in window/global scope', () => {
      (window as IWindow).self = (window as IWindow).top;
      // tslint:disable-next-line: no-string-literal
      (window as IWindow)['PARENT_HTTP_CLIENT'] = jasmine.createSpy('HttpClientSpy');

      // tslint:disable-next-line: no-string-literal
      expect(serviceManager.httpClient).toEqual((window as IWindow)['PARENT_HTTP_CLIENT']);
    });

    it('should return the httpClient of the parent frame', () => {
      (window as IWindow).self = {};
      // tslint:disable-next-line: no-string-literal
      (window as IWindow).parent['PARENT_HTTP_CLIENT'] = jasmine.createSpy('HttpClientSpy');

      // tslint:disable-next-line: no-string-literal
      expect(serviceManager.httpClient).toEqual((window as IWindow).parent['PARENT_HTTP_CLIENT']);
    });

    it('should return undefined when isn\'t defined any httpClient in window', () => {
      expect(serviceManager.httpClient).toBeFalsy();
    });
  });

  describe('get appConfig()', () => {
    it('should return the appConfig defined in window', () => {
      // tslint:disable-next-line: no-string-literal
      (window as IWindow)['appConfig'] = jasmine.createSpy('appConfig');

      // tslint:disable-next-line: no-string-literal
      expect(serviceManager.appConfig).toEqual((window as IWindow)['appConfig']);
    });

    it('should return a default appConfig object when it isn\'t defined in window/global scope', () => {
      expect(serviceManager.appConfig.resourceUrl).toEqual('/');
    });
  });
});

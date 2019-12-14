import { IWidgetRuntime, widgetRuntime } from '@impartner/widget-runtime';

import { EndpointLocatorService } from './endpoint-locator.service';

describe('endpoint-locator.service.ts', () => {
  let serviceUnderTest: EndpointLocatorService;
  let widgetRuntimeStub: IWidgetRuntime;

  beforeEach(() => {
    widgetRuntimeStub = widgetRuntime;
    serviceUnderTest = new EndpointLocatorService(widgetRuntimeStub);
  });

  describe('getApiUrl()', () => {
    it('should return an empty string when httpClient is defined', () => {
      spyOnProperty(widgetRuntimeStub.services, 'httpClient', 'get').and.returnValue(
        'https://fakeenpoint.com/api'
      );

      const url = serviceUnderTest.getApiUrl();

      expect(url).toEqual('');
    });

    it('should return a fallback url when httpClient isn\' defined', () => {
      spyOnProperty(widgetRuntimeStub.services, 'httpClient', 'get').and.returnValue(undefined);

      const url = serviceUnderTest.getApiUrl();

      expect(url).toEqual('http://localhost:9876/api');
    });
  });
});

import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { IHttpClient, widgetRuntime } from '@impartner/widget-runtime';
import { ColDef, IGetRowsParams } from 'ag-grid-community';
import { CookieService } from 'ngx-cookie-service';

import { PrmObject } from 'src/app/core/enums';
import { environment } from 'src/environments/environment';
import { CORE_TOKENS } from '../../../../core/constants';
import { HttpClientAdapter } from '../../adapters';
import { httpClientFactory } from '../../factories';
import { fieldDefinitions } from '../../spec-fixtures';
import { EndpointLocatorService } from '../endpoint-locator.service';
import { RequestCreatorService } from './request-creator.service';

describe('request-creator.service.ts', () => {
  let serviceUnderTest: RequestCreatorService;
  let httpClient: jasmine.SpyObj<IHttpClient>;
  let httpAngularClient: jasmine.SpyObj<HttpClient>;
  let prmObject: PrmObject;
  let params: Partial<IGetRowsParams>;
  let columnsToShow: Partial<ColDef>[];

  beforeAll(() => {
    httpAngularClient = jasmine.createSpyObj('httpAngularClient', ['get', 'post']);
  });

  describe('constructor', () => {
    it('should create a http header with cookies setted', () => {
      environment.production = true;

      TestBed.configureTestingModule({
        providers: [
          RequestCreatorService,
          CookieService,
          { provide: HttpClient, useValue: httpAngularClient },
          HttpClientAdapter,
          EndpointLocatorService,
          { provide: CORE_TOKENS.WidgetRuntime, useValue: widgetRuntime },
          {
            provide: CORE_TOKENS.HttpClient,
            useFactory: httpClientFactory,
            deps: [HttpClientAdapter, CORE_TOKENS.WidgetRuntime]
          }
        ]
      });

      const cookieService: CookieService = TestBed.get(CookieService);
      cookieService.set('RV.Dev.Auth', 'testtoken');
      cookieService.set('RV.Dev.CsrfToken', 'c8da7780bf1acc6a10f18bdc8fbe4660');
      cookieService.set('RV.Dev.CultureId', 'en');
      cookieService.set('TS01c6638a', '985296264d712b18fac0e7b3409');

      const serviceUnderTestInProductionMode: RequestCreatorService = TestBed.get(
        RequestCreatorService
      );

      const cookieValue = serviceUnderTestInProductionMode.httpHeaders.get('cookie');
      expect(cookieValue).toContain('RV.Dev.Auth=testtoken');
      expect(cookieValue).toContain(
        'RV.Dev.Auth=testtoken; RV.Dev.CsrfToken=c8da7780bf1acc6a10f18bdc8fbe4660'
      );
    });

    it("shouldn't create a cookie header", () => {
      environment.production = false;

      TestBed.configureTestingModule({
        providers: [
          RequestCreatorService,
          CookieService,
          { provide: HttpClient, useValue: httpAngularClient },
          HttpClientAdapter,
          EndpointLocatorService,
          { provide: CORE_TOKENS.WidgetRuntime, useValue: widgetRuntime },
          { provide: CORE_TOKENS.WidgetRuntime, useValue: widgetRuntime },
          {
            provide: CORE_TOKENS.HttpClient,
            useFactory: httpClientFactory,
            deps: [HttpClientAdapter, CORE_TOKENS.WidgetRuntime]
          }
        ]
      });

      const cookieService: CookieService = TestBed.get(CookieService);
      cookieService.set('RV.Dev.Auth', 'testtoken');
      cookieService.set('RV.Dev.CsrfToken', 'c8da7780bf1acc6a10f18bdc8fbe4660');
      cookieService.set('RV.Dev.CultureId', 'en');
      cookieService.set('TS01c6638a', '985296264d712b18fac0e7b3409');

      const serviceUnderTestInProductionMode: RequestCreatorService = TestBed.get(
        RequestCreatorService
      );

      expect(serviceUnderTestInProductionMode.httpHeaders).toBeFalsy();
    });
  });

  describe('methods', () => {
    beforeEach(() => {
      httpClient = jasmine.createSpyObj<IHttpClient>('HttpClient', ['get', 'post']);
      httpClient.get.and.returnValue(Promise.resolve({ data: [] }));
      httpClient.post.and.returnValue(Promise.resolve({ data: [] }));

      TestBed.configureTestingModule({
        providers: [
          RequestCreatorService,
          CookieService,
          { provide: HttpClient, useValue: httpAngularClient },
          HttpClientAdapter,
          EndpointLocatorService,
          { provide: CORE_TOKENS.WidgetRuntime, useValue: widgetRuntime },
          {
            provide: CORE_TOKENS.HttpClient,
            useValue: httpClient
          }
        ]
      });

      serviceUnderTest = TestBed.get(RequestCreatorService);
      prmObject = PrmObject.DEAL;

      params = {
        startRow: 0,
        endRow: 10,
        sortModel: [{ colId: 'name', sort: 'asc' }]
      };

      columnsToShow = [{ field: 'name' }, { field: 'customer.name' }, { field: 'account' }];
    });

    describe('getGridData()', () => {
      it('should have setted a valid endpoint url', () => {
        serviceUnderTest.getGridData(prmObject, params as IGetRowsParams, columnsToShow);
        const url = httpClient.post.calls.mostRecent().args[0];

        expect(url).toContain('objects/v1/Deal');
      });
    });

    describe('getPrmObjects()', () => {
      it('should call the REST URL that return the list of PRM Objects', () => {
        serviceUnderTest.getPrmObjects();

        const url = httpClient.get.calls.mostRecent().args[0];

        expect(url).toContain('objects/v1/_describe');
      });
    });

    describe('getPrmObjectMetadata()', () => {
      it('should call the REST URL that return the metadata of a PRM Object', () => {
        serviceUnderTest.getPrmObjectMetadata('Deal');

        const url = httpClient.get.calls.mostRecent().args[0];

        expect(url).toContain('objects/v1/Deal/_describe');
      });

      it('should return a list of field definitions', async () => {
        httpClient.get.and.returnValue(Promise.resolve({ data: fieldDefinitions }));

        const fieldDefinition = await serviceUnderTest.getPrmObjectMetadata('Deal');

        const namesOfFields = fieldDefinition.map(fieldDef => fieldDef.name);

        expect(namesOfFields).toContain('Company');
        expect(namesOfFields).toContain('Source');
        expect(namesOfFields).toContain('Updated');
      });

      it("shouldn't return a list of fields that have as type 'list'", async () => {
        httpClient.get.and.returnValue(Promise.resolve({ data: fieldDefinitions }));

        const fieldDefinition = await serviceUnderTest.getPrmObjectMetadata('Deal');

        const namesOfFields = fieldDefinition.map(fieldDef => fieldDef.name);

        expect(namesOfFields).not.toContain('Accounts');
      });
    });
  });
});

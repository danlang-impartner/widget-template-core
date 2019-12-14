import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { IGetRowsParams } from 'ag-grid-community';

import { DEFAULT_COLUMNS_BY_PRM_OBJECT } from '../config';
import { IGridConfiguration } from '../interfaces';
import { fieldDefinitions, testDataOfPrmObject } from '../spec-fixtures';
import { PrmObjectService } from './prm-object.service';
import { RequestCreatorService } from './request-creator.service.ts';

describe('prm-object.service.ts', () => {
  let serviceUnderTest: PrmObjectService;
  let requestCreatorServiceMock: jasmine.SpyObj<RequestCreatorService>;
  let gridSettings: IGridConfiguration;

  beforeEach(() => {
    requestCreatorServiceMock = jasmine.createSpyObj('RequestCreatorService', [
      'getPrmObjectMetadata',
      'getGridData'
    ]);

    requestCreatorServiceMock.getPrmObjectMetadata.and.returnValue(
      Promise.resolve(fieldDefinitions)
    );

    requestCreatorServiceMock.getGridData.and.returnValue(Promise.resolve(testDataOfPrmObject));

    TestBed.configureTestingModule({
      providers: [
        PrmObjectService,
        { provide: RequestCreatorService, useValue: requestCreatorServiceMock }
      ]
    });

    serviceUnderTest = TestBed.get(PrmObjectService);
    gridSettings = {
      businessObjectName: 'Deal',
      columnsToShow: DEFAULT_COLUMNS_BY_PRM_OBJECT.Deal,
      resultsPerPage: [10]
    };
  });

  describe('getColumnDefinition()', () => {
    it('should return a list of columns to be showed in grid', async () => {
      const columnDef = await serviceUnderTest.getColumnDefinition(gridSettings);
      const fieldList = columnDef.map(colDef => colDef.field);

      expect(fieldList).toContain('name');
      expect(fieldList).toContain('company.name');
      expect(fieldList).toContain('source');
      expect(fieldList).toContain('amount');
      expect(fieldList).toContain('probability');
      expect(fieldList).toContain('stage.name');
      expect(fieldList).toContain('closeDate');
    });

    it('should sort in the same order setted in gridSettings', async () => {
      gridSettings.columnsToShow = ['name', 'source', 'company.name'];
      const columnDef = await serviceUnderTest.getColumnDefinition(gridSettings);
      const fieldList = columnDef.map(colDef => colDef.field);

      expect(fieldList[0]).toBe('name');
      expect(fieldList[1]).toBe('source');
      expect(fieldList[2]).toBe('company.name');
    });
  });

  describe('getDatasource()', () => {
    let paramsOfQuery: IGetRowsParams;

    beforeEach(() => {
      paramsOfQuery = {
        startRow: 0,
        endRow: 10,
        filterModel: [],
        sortModel: [],
        successCallback: (rows: object[], lastRow: number): void => {},
        failCallback: (): void => {},
        context: {}
      };
    });

    it("should load data from API when call 'getRows' from the returned object", fakeAsync(() => {
      const datasource = serviceUnderTest.getDatasource(gridSettings);

      datasource.getRows(paramsOfQuery);
      tick();

      expect(requestCreatorServiceMock.getGridData).toHaveBeenCalled();
    }));

    it('should call the callback function when fetch the records from API', fakeAsync(() => {
      spyOn(paramsOfQuery, 'successCallback');

      const datasource = serviceUnderTest.getDatasource(gridSettings);

      datasource.getRows(paramsOfQuery);
      tick();

      expect(paramsOfQuery.successCallback).toHaveBeenCalledWith(
        testDataOfPrmObject.results,
        testDataOfPrmObject.count
      );
    }));

    it("should call the 'failCallback' when there is an error in API call", fakeAsync(() => {
      requestCreatorServiceMock.getGridData.and.returnValue(Promise.reject('Simulated error'));
      spyOn(paramsOfQuery, 'failCallback');

      const datasource = serviceUnderTest.getDatasource(gridSettings);

      datasource.getRows(paramsOfQuery);
      tick();

      expect(paramsOfQuery.failCallback).toHaveBeenCalled();
    }));
  });
});

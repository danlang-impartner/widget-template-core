import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IDatasource, IGetRowsParams } from 'ag-grid-community';

import { DEFAULT_COLUMNS_BY_PRM_OBJECT } from '../../config';
import { IGridConfiguration } from '../../interfaces';
import { PrmObjectService } from '../../services';
import { columnDefinitionOfDeal, testDataOfPrmObject } from '../../spec-fixtures';
import { GridViewComponent } from './grid-view.component';

describe('grid-view-component.ts', () => {
  let component: GridViewComponent;
  let fixture: ComponentFixture<GridViewComponent>;
  let prmObjectServiceMock: jasmine.SpyObj<PrmObjectService>;
  let widgetConfig: IGridConfiguration;

  beforeEach(async(() => {
    prmObjectServiceMock = jasmine.createSpyObj<PrmObjectService>('PrmObjectService', [
      'getColumnDefinition',
      'getPaginationPageSize',
      'getDatasource',
      'setPaginationPageSize',
      'setFieldsToShow'
    ]);

    prmObjectServiceMock.getColumnDefinition.and.returnValue(
      Promise.resolve(columnDefinitionOfDeal)
    );

    prmObjectServiceMock.getDatasource.and.returnValue({
      getRows: (params: IGetRowsParams): void => {
        params.successCallback(testDataOfPrmObject.results, 10);
      }
    } as IDatasource);

    TestBed.configureTestingModule({
      declarations: [GridViewComponent],
      providers: [{ provide: PrmObjectService, useValue: prmObjectServiceMock }],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    widgetConfig = {
      businessObjectName: 'Deal',
      columnsToShow: ['company.name', 'source', 'amount'],
      resultsPerPage: [10],
      rowFilters: []
    };
  });

  describe('set widgetConfig()', () => {
    it('should be setted', () => {
      component.widgetConfig = JSON.stringify(widgetConfig);

      const parsedWidgetConfig = JSON.parse(component.widgetConfig) as IGridConfiguration;
      expect(parsedWidgetConfig.businessObjectName).toBe('Deal');
      expect(parsedWidgetConfig.columnsToShow).toEqual(widgetConfig.columnsToShow);
      expect(parsedWidgetConfig.resultsPerPage).toContain(10);
    });

    it('should set default value when resultsPerPage is empty', () => {
      delete widgetConfig.resultsPerPage;
      component.widgetConfig = JSON.stringify(widgetConfig);

      const parsedWidgetConfig = JSON.parse(component.widgetConfig) as IGridConfiguration;
      expect(parsedWidgetConfig.resultsPerPage).toEqual([25, 50, 100, 200]);
    });

    it('should set default value when columnsToShow is empty', () => {
      delete widgetConfig.columnsToShow;
      component.widgetConfig = JSON.stringify(widgetConfig);

      const parsedWidgetConfig = JSON.parse(component.widgetConfig) as IGridConfiguration;
      expect(parsedWidgetConfig.columnsToShow).toEqual(DEFAULT_COLUMNS_BY_PRM_OBJECT.Deal);
    });

    it('should get column definition from REST API', () => {
      component.widgetConfig = JSON.stringify(widgetConfig);

      expect(prmObjectServiceMock.getColumnDefinition).toHaveBeenCalledWith(widgetConfig);
    });
  });
});

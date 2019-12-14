import { DebugElement, NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { IConfigChangedEvent, widgetRuntime } from '@impartner/widget-runtime';
import { ColDef, IDatasource, IGetRowsParams } from 'ag-grid-community';

import { FakeTranslationModule } from 'src/app/core/test-utils';
import { GridComponent } from '../../grid/containers/grid.component';
import { DEFAULT_COLUMNS_BY_PRM_OBJECT } from '../config';
import { IDataSourceDefinition, IGridConfiguration } from '../interfaces';
import { PrmObjectService } from '../services/prm-object.service';
import {
  columnDefinitionOfDeal,
  columnDefinitionsSample,
  testDataOfPrmObject
} from '../spec-fixtures';
import { PrmObjectGridComponent } from './prm-object-grid.component';

describe('prm-object-grid.component.ts', () => {
  let component: PrmObjectGridComponent;
  let fixture: ComponentFixture<PrmObjectGridComponent>;
  let debugElement: DebugElement;
  let prmObjectServiceMock: jasmine.SpyObj<PrmObjectService>;
  let sampleGridSettings: IGridConfiguration;
  let gridDebugElement: DebugElement;
  let gridComponent: GridComponent;
  let gridConfiguration: IGridConfiguration;

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
      declarations: [PrmObjectGridComponent],
      imports: [FakeTranslationModule],
      providers: [
        PrmObjectService,
        { provide: PrmObjectService, useValue: prmObjectServiceMock },
        { provide: 'IEventBus', useValue: widgetRuntime.eventBus }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrmObjectGridComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;

    gridConfiguration = component.widgetConfig as IGridConfiguration;
    gridDebugElement = debugElement.query(By.css('w-impartner-grid'));
    gridComponent = gridDebugElement.nativeElement;
    sampleGridSettings = {
      businessObjectName: 'Deal',
      columnsToShow: DEFAULT_COLUMNS_BY_PRM_OBJECT.Deal,
      resultsPerPage: [15, 25, 30],
      rowFilters: []
    };

    fixture.detectChanges();
  });

  describe('set widgetConfig()', () => {
    it('should update the businessObject and pageSize of component', () => {
      component.widgetConfig = JSON.stringify(sampleGridSettings);
      const updatedGridConfig: IGridConfiguration = (component.widgetConfig as unknown) as IGridConfiguration;

      expect(updatedGridConfig.businessObjectName).toBe('Deal');
      expect(updatedGridConfig.resultsPerPage).toContain(15);
    });

    it('should throw error for invalid JSON value', () => {
      expect(() => {
        component.widgetConfig = '[{test: error}]';
      }).toThrowError();
    });

    it('should set default columns to show when this property is empty', () => {
      sampleGridSettings.columnsToShow = [];
      component.widgetConfig = JSON.stringify(sampleGridSettings);

      expect(gridConfiguration.columnsToShow).toEqual(DEFAULT_COLUMNS_BY_PRM_OBJECT.Deal);
    });

    it('should update the column definition with the columns of prm object', () => {
      component.widgetConfig = JSON.stringify(sampleGridSettings);

      expect(prmObjectServiceMock.getColumnDefinition).toHaveBeenCalledWith(sampleGridSettings);
    });

    it('should set the datasource object in grid widget', async(async () => {
      component.widgetConfig = JSON.stringify(sampleGridSettings);
      fixture.detectChanges();
      await fixture.whenStable();

      expect(prmObjectServiceMock.getColumnDefinition).toHaveBeenCalledWith(sampleGridSettings);
    }));

    it('should emit an event when configuration changed', (done: Function) => {
      component.configurationChanged.subscribe((event: IConfigChangedEvent<any>) => {
        expect(event.configuration).toEqual(sampleGridSettings);
        done();
      });

      component.widgetConfig = JSON.stringify(sampleGridSettings);
      component.widgetConfig = JSON.stringify(sampleGridSettings);
      fixture.detectChanges();
    });
  });

  describe('set columnDefinitions()', () => {
    it('should update the column definitions in grid widget', fakeAsync(() => {
      component.columnDefinition = columnDefinitionsSample;
      fixture.detectChanges();
      tick();

      const metadataGrid: ColDef[] = JSON.parse(gridComponent.metadata);
      const fieldList = metadataGrid.map(columnDefinition => columnDefinition.field);
      expect(fieldList).toContain('company.name');
    }));
  });
});

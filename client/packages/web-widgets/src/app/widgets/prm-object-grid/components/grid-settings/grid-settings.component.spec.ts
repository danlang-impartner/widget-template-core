import { DebugElement, NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { widgetRuntime } from '@impartner/widget-runtime';
import { ColDef } from 'ag-grid-community';

import { CORE_TOKENS } from 'src/app/core/constants';
import { FakeTranslationModule } from 'src/app/core/test-utils';
import { PopupModule } from 'src/app/shared';
import { IDataSourceDefinition, IGridConfiguration } from '../../interfaces';
import { columnDefinitionsSample } from '../../spec-fixtures';
import { GridSettingsComponent } from './grid-settings.component';

describe('grid-settings.component.ts', () => {
  let component: GridSettingsComponent;
  let fixture: ComponentFixture<GridSettingsComponent>;
  let datasourceDefinition: IDataSourceDefinition;
  let debugElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [GridSettingsComponent],
      imports: [PopupModule, FakeTranslationModule],
      providers: [{ provide: CORE_TOKENS.IEventBus, useValue: widgetRuntime.eventBus }],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridSettingsComponent);
    debugElement = fixture.debugElement;
    component = fixture.componentInstance;

    datasourceDefinition = {
      widgetId: 1,
      columnDefinition: columnDefinitionsSample,
      widgetConfig: {
        businessObjectName: 'Deal',
        columnsToShow: ['name', 'company.name', 'source'],
        resultsPerPage: [15]
      }
    };

    fixture.detectChanges();
  });

  describe('set datasourceDefinition()', () => {
    it('should update the gridConfiguration of the component', () => {
      component.datasourceDefinition = datasourceDefinition;

      expect(component.gridConfiguration).toEqual(
        datasourceDefinition.widgetConfig as IGridConfiguration
      );
    });

    it('should update the columnDefinition of the component', () => {
      component.datasourceDefinition = datasourceDefinition;

      expect(component.columnDefinition).toEqual(datasourceDefinition.columnDefinition);
    });
  });

  describe('hideOrUnhideField()', () => {
    let columnDefinition: ColDef;

    beforeEach(() => {
      component.datasourceDefinition = datasourceDefinition;
      columnDefinition = {
        headerName: 'Created',
        field: 'created',
        colId: 'created',
        filter: 'agDateColumnFilter',
        hide: true
      };
    });

    it('should show first the column checked as visible', () => {
      component.hideOrUnhideField(columnDefinition);
      fixture.detectChanges();

      expect(component.datasourceDefinition.columnDefinition[0].hide).toBe(false);
      expect(component.datasourceDefinition.columnDefinition[0].field).toBe('created');
    });

    it('should position as last the column checked as hidden', () => {
      columnDefinition.hide = false;
      component.hideOrUnhideField(columnDefinition);

      fixture.detectChanges();

      const lastColumnDefinition = component.datasourceDefinition.columnDefinition.pop() as ColDef;
      expect(lastColumnDefinition.field).toBe('created');
    });

    it('should update the columnsToShow in the widget configuration', () => {
      component.hideOrUnhideField(columnDefinition);
      fixture.detectChanges();

      const expectedColumnsToShow = ['created', 'accounts', 'closeDate', 'company.name'];

      expect(component.gridConfiguration.columnsToShow).toEqual(expectedColumnsToShow);
    });

    it('should update the prm grid widget', () => {
      component.hideOrUnhideField(columnDefinition);
      fixture.detectChanges();

      expect(component.datasourceDefinition.widgetConfig).toEqual(
        datasourceDefinition.widgetConfig
      );
    });
  });

  describe('onOrderChanged', () => {
    beforeEach(() => {
      component.datasourceDefinition = datasourceDefinition;
    });

    it('should change the order of the columns to show in the config', () => {
      const columnDefinitionDifferentOrder = [
        { ...component.columnDefinition[1] },
        { ...component.columnDefinition[0] },
        { ...component.columnDefinition[2] }
      ];

      component.columnDefinition = columnDefinitionDifferentOrder;
      component.onOrderChanged(new Event(''));

      expect(
        (component.datasourceDefinition.widgetConfig as IGridConfiguration).columnsToShow
      ).toEqual(['closeDate', 'accounts', 'company.name']);
    });
  });
});

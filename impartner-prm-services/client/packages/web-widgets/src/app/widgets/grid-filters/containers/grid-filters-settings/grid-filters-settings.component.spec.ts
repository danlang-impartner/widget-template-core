import { CUSTOM_ELEMENTS_SCHEMA, DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import {
  ImpartnerWidgetTypes,
  IShowConfigRequestedEvent,
  SystemEvents,
  widgetRuntime
} from '@impartner/widget-runtime';

import { CORE_TOKENS } from 'src/app/core/constants';
import { IRowFilter } from '../../../../core';
import { FilterTab, OperatorSymbol } from '../../enums';
import { IPrmGridComponent, ITabConfig } from '../../interfaces';
import { GridFiltersSettingsComponent } from './grid-filters-settings.component';

describe('grid-filters-settings.component.ts', () => {
  let component: GridFiltersSettingsComponent;
  let fixture: ComponentFixture<GridFiltersSettingsComponent>;
  let debugElement: DebugElement;
  let gridInstance: IPrmGridComponent;
  let tabConfigurationMockup: ITabConfig;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [GridFiltersSettingsComponent],
      imports: [FakeTranslationModule],
      providers: [
        {
          provide: CORE_TOKENS.IEventBus,
          useValue: widgetRuntime.eventBus
        }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridFiltersSettingsComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;

    gridInstance = {
      columnDefinition: [
        { colId: 'tst01', header: 'test 01', hiden: false },
        { colId: 'tst02', header: 'test 02', hiden: false },
        { colId: 'tst03', header: 'test 03', hiden: false }
      ],
      widgetConfig: {
        businessObjectName: 'PrmObject01',
        columnsToShow: ['tstColumn01', 'tstColumn02', 'tstColumn03'],
        rowFilters: []
      }
    };

    tabConfigurationMockup = {
      id: 1,
      name: 'test tab 1',
      gridConfig: gridInstance.widgetConfig
    };

    widgetRuntime.eventBus.emit<IShowConfigRequestedEvent<ITabConfig>>(
      SystemEvents.ShowConfigRequested,
      {
        type: ImpartnerWidgetTypes.ImpartnerGridFilters,
        configuration: tabConfigurationMockup,
        widgetId: 11
      }
    );

    widgetRuntime.eventBus.emit<IShowConfigRequestedEvent<IPrmGridComponent>>(
      SystemEvents.ShowConfigRequested,
      {
        type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
        configuration: gridInstance,
        widgetId: 1
      }
    );

    fixture.detectChanges();
  });

  describe('constructor()', () => {
    it('should listen to ShowConfigRequested event, and update the tab configuration', () => {
      expect(component.tabConfig).toEqual(tabConfigurationMockup);
    });
  });

  describe('get rowFilters()', () => {
    it("should return a default value when there isn't any rowFilters", () => {
      const expectedDefaultRowFilter: IRowFilter = {
        id: 0,
        booleanOperator: 'or',
        fact: 'tst01',
        operator: '=',
        value: ['']
      };

      const rowFilters = component.rowFilters;
      expect(rowFilters).toEqual([expectedDefaultRowFilter]);
    });

    it('should return the rowFilters configured inside the grid component', () => {
      const expectedRowFilter: IRowFilter = {
        id: 1,
        booleanOperator: 'and',
        fact: 'tst02',
        operator: '!=',
        value: ['test Value']
      };

      gridInstance.widgetConfig.rowFilters = [expectedRowFilter];

      widgetRuntime.eventBus.emit<IShowConfigRequestedEvent<IPrmGridComponent>>(
        SystemEvents.ShowConfigRequested,
        {
          type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
          configuration: gridInstance,
          widgetId: 1
        }
      );

      const rowFilters = component.rowFilters;
      expect(rowFilters).toEqual([expectedRowFilter]);
    });
  });

  describe('onBusinessObjectChanged()', () => {
    it('should update the widgetConfig in the prm grid widget instance', () => {
      component.onBusinessObjectChanged('PrmObject02');
      expect(gridInstance.widgetConfig.businessObjectName).toEqual('PrmObject02');
      expect(gridInstance.widgetConfig.columnsToShow).toEqual([]);
    });
  });

  describe('onTabChanged()', () => {
    it('should show filter rows panel if tab is Filter', () => {
      component.onTabChanged(FilterTab.Filter);
      const prmDatagridSettingsPanel = debugElement.query(By.css('.filter-rows-panel'));

      expect(prmDatagridSettingsPanel).not.toBeNull();
    });

    it('should show prm datagrid settings panel if tab is Datasource', () => {
      component.onTabChanged(FilterTab.Datasource);
      const prmDatagridSettingsPanel = debugElement.query(
        By.css('w-impartner-prm-object-grid-settings')
      );
      const classesPrmDatagirdSettingsPanel = prmDatagridSettingsPanel.classes;
      const enabledClases = Object.keys(prmDatagridSettingsPanel.classes).filter(
        className => classesPrmDatagirdSettingsPanel[className]
      );

      expect(enabledClases).toContain('show');
    });
  });

  describe('updateFilters()', () => {
    it("should add the updated filter if isn't exist into prm grid widgetConfig", () => {
      const updatedRowFilter: IRowFilter = {
        id: 0,
        booleanOperator: 'and',
        fact: 'tst01',
        operator: OperatorSymbol.Equals,
        value: ['another test value']
      };
      const expectedWidgetConfig = JSON.stringify({
        ...gridInstance.widgetConfig,
        rowFilters: [updatedRowFilter]
      });
      component.updateFilters(updatedRowFilter);
      expect((gridInstance.widgetConfig as unknown) as string).toEqual(expectedWidgetConfig);
    });

    it('should update the filter if exists into prm grid widgetConfig', () => {
      gridInstance.widgetConfig.rowFilters = [
        {
          id: 0,
          booleanOperator: 'and',
          fact: 'tst01',
          operator: '=',
          value: ['initial test value']
        }
      ];
      const updatedRowFilter: IRowFilter = {
        id: 0,
        booleanOperator: 'and',
        fact: 'tst01',
        operator: '=',
        value: ['initial test value', 'another test value']
      };
      const expectedWidgetConfig = JSON.stringify({
        ...gridInstance.widgetConfig,
        rowFilters: [updatedRowFilter]
      });
      component.updateFilters(updatedRowFilter);
      expect((gridInstance.widgetConfig as unknown) as string).toEqual(expectedWidgetConfig);
    });
  });

  describe('addFilter()', () => {
    it('should add a new filter to rowFilters list', () => {
      const newRowFilter: IRowFilter = {
        id: 1,
        booleanOperator: 'or',
        fact: 'tst01',
        operator: '=',
        value: ['']
      };
      const expectedWidgetConfig = JSON.stringify({
        ...gridInstance.widgetConfig,
        rowFilters: [{ ...newRowFilter, id: 0 }, newRowFilter]
      });

      component.addFilter();
      expect((gridInstance.widgetConfig as unknown) as string).toEqual(expectedWidgetConfig);
    });
  });
});

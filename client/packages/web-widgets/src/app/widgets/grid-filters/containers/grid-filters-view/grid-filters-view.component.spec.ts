import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FakeTranslationModule } from 'src/app/core/test-utils';
import { ShowTabsComponent } from '../../components';
import { DEFAULT_TABS_CONFIG } from '../../constants';
import { IGridFiltersConfig } from '../../interfaces';
import { GridFiltersViewComponent } from './grid-filters-view.component';

describe('grid-filters-view.component.ts', () => {
  let component: GridFiltersViewComponent;
  let fixture: ComponentFixture<GridFiltersViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [GridFiltersViewComponent, ShowTabsComponent],
      imports: [FakeTranslationModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridFiltersViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('constructor()', () => {
    it('should create an instance', () => {
      expect(component).toBeTruthy();
    });
  });

  describe('set widgetConfig()', () => {
    it('should update the grid filters configuration when a string is setted', () => {
      const gridFiltersConfiguration: IGridFiltersConfig = {
        tabs: [
          {
            id: 1,
            name: 'test tab 1',
            gridConfig: {}
          }
        ]
      };

      component.widgetConfig = JSON.stringify(gridFiltersConfiguration);
      expect(component.gridFiltersConfig).toEqual(gridFiltersConfiguration);
    });

    it("should set the grid filters configuration to default when there aren't tabs configuration", () => {
      component.widgetConfig = JSON.stringify({});
      expect(component.gridFiltersConfig).toEqual(DEFAULT_TABS_CONFIG);
    });
  });
});

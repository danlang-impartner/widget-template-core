import { CUSTOM_ELEMENTS_SCHEMA, DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { PrmObject } from 'src/app/core/enums';
import { SelectListOption } from 'src/app/shared/select/interfaces';
import { FilterTab } from '../../enums';
import { DefineDatasourceComponent } from './define-datasource.component';

describe('define-datasource.component.ts', () => {
  let component: DefineDatasourceComponent;
  let fixture: ComponentFixture<DefineDatasourceComponent>;
  let debugElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DefineDatasourceComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DefineDatasourceComponent);
    debugElement = fixture.debugElement;
    component = fixture.componentInstance;
    component.businessObjectName = PrmObject.DEAL;
    fixture.detectChanges();
  });

  describe('businessObjectChanged()', () => {
    it('should emit an event, notifying about the change in the business object', () => {
      const optionMock: SelectListOption<string> = {
        label: 'Opportunity',
        value: 'Opportunity'
      };
      let optionEmitted = '';
      component.businessObjectChangeEvent.subscribe((value: string) => (optionEmitted = value));

      component.businessObjectChanged(optionMock);

      expect(optionEmitted).toBe('Opportunity');
    });
  });

  describe('markTabAsSelected()', () => {
    it("should mark 'Datasource' tab as active", () => {
      let selectedTab: FilterTab | undefined;
      component.tabSelectedEvent.subscribe((tab: FilterTab) => (selectedTab = tab));
      const datasourceTab = debugElement.query(By.css('.w-form-field'));
      datasourceTab.triggerEventHandler('click', {});

      expect(selectedTab).toBe(FilterTab.Datasource);
    });

    it("should mark 'Filter' tab as active", () => {
      let selectedTab: FilterTab | undefined;
      component.tabSelectedEvent.subscribe((tab: FilterTab) => (selectedTab = tab));
      const filterTab = debugElement.query(By.css('.w-filter'));
      filterTab.triggerEventHandler('click', {});

      expect(selectedTab).toBe(FilterTab.Filter);
    });
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ITabConfig } from '../../interfaces';
import { ShowTabsComponent } from './show-tabs.component';

describe('ShowTabsComponent', () => {
  let component: ShowTabsComponent;
  let fixture: ComponentFixture<ShowTabsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ShowTabsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('set tabs()', () => {
    it('should set the first tab as selected', () => {
      component.tabs = [
        {
          id: 1,
          name: 'test',
          gridConfig: {}
        },
        {
          id: 2,
          name: 'test2',
          gridConfig: {}
        }
      ];

      expect(component.selectedTabIndex).toBe(1);
    });
  });

  describe('onTabSelected()', () => {
    it('should emit an event with the tab selected', () => {
      spyOn(component.tabSelected, 'emit');
      const tabSelected: ITabConfig = {
        id: 1,
        name: 'test tab',
        gridConfig: {}
      };

      component.onTabSelected(tabSelected);
      expect(component.tabSelected.emit).toHaveBeenCalledWith(tabSelected);
    });
  });
});

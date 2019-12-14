import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {
  IEventBus,
  ImpartnerWidgetTypes,
  IShowConfigRequestedEvent
} from '@impartner/widget-runtime';

import { CORE_TOKENS } from 'src/app/core/constants';
import { FakeTranslationModule } from 'src/app/core/test-utils';
import { ShowTabsComponent } from '../../components';
import { ITabConfig } from '../../interfaces';
import { GridFiltersEditComponent } from './grid-filters-edit.component';

describe('grid-filters-edit.component.ts', () => {
  let component: GridFiltersEditComponent;
  let fixture: ComponentFixture<GridFiltersEditComponent>;
  let eventBus: jasmine.SpyObj<IEventBus>;

  beforeEach(async(() => {
    eventBus = jasmine.createSpyObj<IEventBus>('eventbus', ['addEventListener']);
    TestBed.configureTestingModule({
      declarations: [GridFiltersEditComponent, ShowTabsComponent],
      imports: [FakeTranslationModule],
      providers: [
        {
          provide: CORE_TOKENS.IEventBus,
          useValue: eventBus
        }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridFiltersEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('setAsActiveGridForSettings()', () => {
    it('should emit show configuration panel event', (done: DoneFn) => {
      component.showConfigEvent.subscribe((event: IShowConfigRequestedEvent<ITabConfig>) => {
        expect(event.type).toBe(ImpartnerWidgetTypes.ImpartnerGridFilters);
        done();
      });

      component.setAsActiveGridForSettings();
    });
  });
});

import { CUSTOM_ELEMENTS_SCHEMA, DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { IItemListInterface } from '../interfaces';
import { MultioptionListComponent } from './multioption-list.component';

describe('multioption-list.component.ts', () => {
  let component: MultioptionListComponent;
  let fixture: ComponentFixture<MultioptionListComponent>;
  let debugElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MultioptionListComponent],
      imports: [],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MultioptionListComponent);
    debugElement = fixture.debugElement;
    component = fixture.componentInstance;
    component.options = [
      { label: 'test01', value: 'test01' },
      { label: 'test02', value: 'test02' },
      { label: 'test03', value: 'test03' },
      { label: 'test04', value: 'test04' }
    ];
    fixture.detectChanges();
  });

  describe('removeItem()', () => {
    it('should remove the option from the list of options', () => {
      let modifiedOptions: IItemListInterface[] = [];
      component.optionsModified.subscribe(
        (options: IItemListInterface[]) => (modifiedOptions = options)
      );
      component.removeItem({ label: 'test03', value: 'test03' });

      const numberOptionsDebugElement = debugElement.queryAll(By.css('.list-item'));
      expect(component.options.length).toBe(3);
      expect(numberOptionsDebugElement.length).toBe(3);
      expect(modifiedOptions.map(option => option.label)).not.toContain('test03');
    });
  });

  describe('updateValue()', () => {
    it('should update the value of the item', () => {
      let modifiedOptions: string[] = [];
      const optionToUpdate = { label: 'test03', value: 'test03' };
      const eventMock = {
        preventDefault: jasmine.createSpy(),
        stopPropagation: jasmine.createSpy(),
        target: {
          value: 'test05'
        }
      };

      component.optionsModified.subscribe(
        (options: IItemListInterface[]) => (modifiedOptions = options.map(option => option.label))
      );

      component.enableEditMode(optionToUpdate);
      fixture.detectChanges();
      const inputDebugElement = debugElement.query(By.css('.input-item'));
      inputDebugElement.triggerEventHandler('keydown.enter', eventMock);
      fixture.detectChanges();

      expect(modifiedOptions).not.toContain('test03');
      expect(modifiedOptions).toContain('test05');
    });
  });

  describe('enableEditMode()', () => {
    it('should enable an option as editable', () => {
      const optionToEnable = { label: 'test03', value: 'test03' };
      component.enableEditMode(optionToEnable);
      const inputDebugElement = debugElement.query(By.css('.input-item'));

      expect(inputDebugElement).not.toBeNull();
      expect(inputDebugElement.nativeElement.value).toBe('test03');
    });
  });
});

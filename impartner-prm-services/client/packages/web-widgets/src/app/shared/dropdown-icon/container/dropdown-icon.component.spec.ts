import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { IDropdownOption } from '../interfaces';
import { DropdownIconComponent } from './dropdown-icon.component';

describe('dropdown-icon.component.ts', () => {
  let component: DropdownIconComponent;
  let fixture: ComponentFixture<DropdownIconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DropdownIconComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DropdownIconComponent);
    component = fixture.componentInstance;
    component.options = [
      { icon: 'test01', label: 'tst01', value: 'tst01' },
      { icon: 'test02', label: 'tst02', value: 'tst02' }
    ];
    component.selectedOption = { icon: 'test01', label: 'tst01', value: 'tst01' };
    fixture.detectChanges();
  });

  describe('toggleOptionsList()', () => {
    it('should show the options dropdown list', () => {
      const eventMockup = jasmine.createSpyObj<Event>('eventMock', ['preventDefault']);

      component.toggleOptionsList(eventMockup);
      fixture.detectChanges();

      const optionsListDegugElement = fixture.debugElement.query(By.css('.options-list'));
      expect(optionsListDegugElement).not.toBeNull();
    });
  });

  describe('valueSelected()', () => {
    it('should mark an option as selected', () => {
      let optionEmited: IDropdownOption = jasmine.createSpyObj<IDropdownOption>('optionMock', ['']);

      component.valueChangedEvent.subscribe(
        (selectedValue: IDropdownOption) => (optionEmited = selectedValue)
      );
      component.valueSelected({ icon: 'test02', label: 'tst02', value: 'tst02' });
      const selectedOptionDegugElement = fixture.debugElement.query(
        By.css('.selected-option .option-label')
      );
      const optionsListDegugElement = fixture.debugElement.query(By.css('.options-list'));
      const selectedOptionElement = selectedOptionDegugElement.nativeElement;

      expect(selectedOptionElement.textContent).toBe('tst02');
      expect(optionsListDegugElement).toBeNull();
      expect(optionEmited.label).toBe('tst02');
    });
  });
});

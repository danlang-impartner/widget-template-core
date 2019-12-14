import { async, ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { SelectComponent } from './select.component';

describe('select.component.spec.ts', () => {
  let fixture: ComponentFixture<SelectComponent>;
  let componentUnderTest: SelectComponent;
  let itemsOfList: { label: string; value: string }[];

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SelectComponent],
      imports: [FormsModule]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectComponent);
    componentUnderTest = fixture.componentInstance;
    componentUnderTest.bindLabel = 'label';
    componentUnderTest.bindValue = 'value';
    itemsOfList = [
      { label: 'test', value: 'test' },
      { label: 'test2', value: 'test2' },
      { label: 'test3', value: 'test3' }
    ];
  });

  describe('set items()', () => {
    it('should show a list of options in the select dropdown', () => {
      componentUnderTest.items = JSON.stringify(itemsOfList);
      fixture.detectChanges();

      const selectComponentDOM: HTMLDivElement = fixture.nativeElement;
      const selectOptions = Array.from(selectComponentDOM.getElementsByTagName('option'));
      const listOfValues = selectOptions.map(item => item.value);

      expect(listOfValues).toContain('test');
      expect(listOfValues).toContain('test2');
      expect(listOfValues).toContain('test3');
    });

    it('should throw an error with a malformed JSON', () => {
      expect(() => {
        componentUnderTest.items = '[test: test]';
      }).toThrowError();
    });
  });

  describe('set default()', () => {
    it('should search the value and set as default in dropdown', () => {
      componentUnderTest.items = JSON.stringify(itemsOfList);
      componentUnderTest.default = 'test3';
      fixture.detectChanges();

      expect(componentUnderTest.defaultValue).toBe('test3');
    });
  });
});

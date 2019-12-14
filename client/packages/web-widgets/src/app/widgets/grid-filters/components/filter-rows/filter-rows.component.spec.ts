import { CUSTOM_ELEMENTS_SCHEMA, DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { IRowFilter } from '../../../../core';
import { OperatorSymbol } from '../../enums';
import { IFieldDefinition } from '../../interfaces';
import { FilterService } from '../../services';
import { FilterRowsComponent } from './filter-rows.component';

describe('filter-rows.component.ts', () => {
  let component: FilterRowsComponent;
  let fixture: ComponentFixture<FilterRowsComponent>;
  let debugElement: DebugElement;
  let filterServiceMockup: jasmine.SpyObj<FilterService>;
  const operatorsMockup = [
    {
      icon: 'equal',
      name: 'Match',
      symbol: OperatorSymbol.Equals
    },
    {
      icon: 'not-equals',
      name: 'NOT Match',
      symbol: OperatorSymbol.NotEqual
    }
  ];

  beforeEach(async(() => {
    filterServiceMockup = jasmine.createSpyObj<FilterService>('filterServiceMockup', [
      'getValidOperators'
    ]);
    filterServiceMockup.getValidOperators.and.returnValue(operatorsMockup);
    TestBed.configureTestingModule({
      declarations: [FilterRowsComponent],
      providers: [{ provide: FilterService, useValue: filterServiceMockup }],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterRowsComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;
    component.title = 'Test title';
    component.fieldDefinitions = [
      {
        colId: 'testFact01',
        headerName: 'Test Fact 01',
        hide: false
      },
      {
        colId: 'testFact02',
        headerName: 'Test Fact 02',
        hide: false
      },
      {
        colId: 'testFact03',
        headerName: 'Test Fact 03',
        hide: false
      }
    ];
    component.rowFilter = {
      id: 1,
      booleanOperator: 'and',
      fact: 'testFact01',
      operator: OperatorSymbol.Equals,
      value: ['testValue']
    };
    fixture.detectChanges();
  });

  describe('fieldChanged()', () => {
    it('should emit an event, notifying about the change in the field', () => {
      let fieldChanged = '';
      component.filterChanged.subscribe((rowFilter: IRowFilter) => (fieldChanged = rowFilter.fact));
      const fieldDefinitionMock: IFieldDefinition = {
        colId: 'testFact02',
        headerName: 'Test Fact 02',
        hide: false
      };

      component.fieldChanged(fieldDefinitionMock);
      expect(fieldChanged).toBe('testFact02');
    });

    it('should update the list of operators', () => {
      const fieldDefinitionMock: IFieldDefinition = {
        colId: 'testFact02',
        headerName: 'Test Fact 02',
        hide: false
      };

      filterServiceMockup.getValidOperators.and.returnValue([
        ...operatorsMockup,
        {
          icon: 'greater-than',
          name: 'Greater than',
          symbol: OperatorSymbol.GreaterThan
        }
      ]);

      component.fieldChanged(fieldDefinitionMock);
      expect(component.operators.map(operator => operator.symbol)).toContain(
        OperatorSymbol.GreaterThan
      );
    });
  });

  describe('operatorChanged()', () => {
    it('should set the operator as selected', () => {
      component.operatorChanged({ value: OperatorSymbol.Equals, label: 'Equals', icon: 'equals' });

      expect(component.selectedOperator.symbol).toBe(OperatorSymbol.Equals);
    });

    it('should emit the change in the selected operator', () => {
      let selectedOperator = '';
      component.filterChanged.subscribe(
        (rowFilter: IRowFilter) => (selectedOperator = rowFilter.operator)
      );
      component.operatorChanged({
        value: OperatorSymbol.NotEqual,
        label: 'NOT Match',
        icon: 'not-equals'
      });

      expect(selectedOperator).toBe(OperatorSymbol.NotEqual);
    });
  });

  describe('filterValueChanged()', () => {
    it('should emit an event, notifying about the change in the value', () => {
      const newValueFilter = 'new value';
      let receivedValueFilter = [''];
      component.filterChanged.subscribe(
        (rowFilter: IRowFilter) => (receivedValueFilter = rowFilter.value)
      );

      const inputDebugElement = debugElement.query(By.css('.input-filter'));
      inputDebugElement.triggerEventHandler('change', { target: { value: newValueFilter } });

      expect(receivedValueFilter).toContain(newValueFilter);
    });
  });

  describe('addNewValue()', () => {
    it('should create new input field', () => {
      const addNewValueLink = debugElement.query(By.css('.another-value-link'));
      addNewValueLink.triggerEventHandler('click', {
        preventDefault: jasmine.createSpy()
      });
      const inputDebugElements = debugElement.queryAll(By.css('.input-filter'));

      expect(inputDebugElements.length).toBe(2);
    });
  });
});

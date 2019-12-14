import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { OperatorSymbol } from '../../../../../grid-filters/enums';
import { Context } from '../context';
import { SettingsRowFilterExpression } from './settings-row-filter-expression';

describe('settings-row-filter-expression.ts', () => {
  let componentUnderTest: SettingsRowFilterExpression;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new SettingsRowFilterExpression();
    const agGridParams: IGetRowsParams = {
      context: {},
      startRow: 0,
      endRow: 10,
      failCallback: jasmine.createSpy('failCallback'),
      filterModel: {},
      sortModel: {},
      successCallback: jasmine.createSpy('successCallback')
    };
    contextMockup = new Context(
      PrmObject.OPPORTUNITY,
      agGridParams,
      ['fieldTst01', 'fieldTst02', 'fieldTst03'],
      []
    );
  });

  describe('interpret()', () => {
    it('should add the row filter to the queryParams filter property', () => {
      contextMockup.rowFilters.push({
        id: 0,
        booleanOperator: 'and',
        fact: 'test01',
        operator: OperatorSymbol.Equals,
        value: ['testValue']
      });
      contextMockup.rowFilters.push({
        id: 1,
        booleanOperator: 'and',
        fact: 'test02',
        operator: OperatorSymbol.Contains,
        value: ['anotherValue']
      });
      contextMockup.rowFilters.push({
        id: 1,
        booleanOperator: 'and',
        fact: 'test03',
        operator: OperatorSymbol.Contains,
        value: ['anotherValue1', 'anotherValue2']
      });

      const expectedOutput =
        "( test01 = 'testValue') and ( test02 like '%anotherValue%') and ( test03 like '%anotherValue1%' OR test03 like '%anotherValue2%')";

      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.filter).toEqual(expectedOutput);
    });
  });
});

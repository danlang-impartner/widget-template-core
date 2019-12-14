import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { GridFilterExpression } from './grid-filter-expression';

describe('grid-filter-expression.ts', () => {
  let componentUnderTest: GridFilterExpression;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new GridFilterExpression();
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
    it('should add the filter to the queryParams filter property', () => {
      contextMockup.agGridParams.filterModel = {
        'company.name': {
          filterType: 'string',
          filter: 'xerox',
          type: 'contains'
        }
      };

      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.filter).toContain("company.name like '%xerox%'");
    });
  });
});

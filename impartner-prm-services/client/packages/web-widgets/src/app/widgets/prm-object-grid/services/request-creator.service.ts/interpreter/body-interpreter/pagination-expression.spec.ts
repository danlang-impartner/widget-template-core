import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { PaginationExpression } from './pagination-expression';

describe('pagination-expression.ts', () => {
  let componentUnderTest: PaginationExpression;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new PaginationExpression();
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
    it('should add the startRow and endRow params to the queryParams output object', () => {
      contextMockup.agGridParams.startRow = 0;
      contextMockup.agGridParams.endRow = 10;

      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.skip).toBe(0);
      expect(contextMockup.queryParams.take).toBe(10);
    });
  });
});

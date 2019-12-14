import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { IOrderByExpression } from '../interfaces';
import { SortByExpression } from './sort-by.expression';

describe('sort-by-expression.ts', () => {
  let componentUnderTest: SortByExpression;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new SortByExpression();
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
    it('should add orderBy criteria to the queryParams output variable', () => {
      contextMockup.agGridParams.sortModel = [
        { display: 'tst01', sort: 'Asc' },
        { display: 'tst02', sort: 'Desc' }
      ];
      const expectedOrder: IOrderByExpression[] = [
        { field: 'tst01', direction: 'Asc' },
        { field: 'tst02', direction: 'Desc' }
      ];

      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.orderBy).toEqual(expectedOrder);
    });

    it("should set an empty array when isn't a defined orderBy criteria", () => {
      contextMockup.agGridParams.sortModel = undefined;
      const expectedOrder: IOrderByExpression[] = [];

      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.orderBy).toEqual(expectedOrder);
    });
  });
});

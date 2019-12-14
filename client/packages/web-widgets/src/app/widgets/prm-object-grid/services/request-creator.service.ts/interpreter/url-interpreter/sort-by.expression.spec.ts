import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { SortByExpression } from './sort-by.expression';

describe('sort-by.expression.ts', () => {
  let sut: SortByExpression;
  let contextObject: Context;
  let rowFilter: Partial<IGetRowsParams>;

  beforeEach(() => {
    sut = new SortByExpression();
    rowFilter = {
      sortModel: [{ colId: 'name', sort: 'asc' }, { colId: 'budget', sort: 'desc' }]
    };
    contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
  });

  describe('interpret()', () => {
    it('should return a sort expression to the REST API URL', () => {
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toBe('&orderBy=name asc,budget desc');
    });

    it("should return an empty string when isn't any 'sort by' in context", () => {
      rowFilter = {};
      contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toBe('');
    });
  });
});

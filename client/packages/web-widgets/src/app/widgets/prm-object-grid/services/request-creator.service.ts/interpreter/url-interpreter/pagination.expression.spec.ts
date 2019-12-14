import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { PaginationExpression } from './pagination.expression';

describe('pagination.expression.ts', () => {
  let sut: PaginationExpression;
  let contextObject: Context;
  let rowFilter: Partial<IGetRowsParams>;

  beforeEach(() => {
    sut = new PaginationExpression();
    rowFilter = {
      startRow: 0,
      endRow: 10
    };
    contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
  });

  describe('interpret()', () => {
    it('should add a pagination filter to the REST API URL', () => {
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toBe('&skip=0&take=10');
    });

    it("should return an empty string when there isn't pagination expression", () => {
      rowFilter = {};
      contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toBe('');
    });
  });
});

import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { FieldsToShowExpression } from './fields-to-show.expression';

describe('fields-to-show.expression.ts', () => {
  let fieldsToShowExpressionObject: FieldsToShowExpression;
  let contextObject: Context;
  let rowFilter: Partial<IGetRowsParams>;

  beforeEach(() => {
    fieldsToShowExpressionObject = new FieldsToShowExpression();
    rowFilter = {};
  });

  describe('interpret()', () => {
    it('should map the fields to the filter field in REST API', () => {
      contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, [
        'name',
        'company.name',
        'account',
        'budget'
      ]);
      const contextInterpreted = fieldsToShowExpressionObject.interpret(contextObject);

      expect(contextInterpreted).toBe('&fields=name,company.name,account,budget');
    });

    it("should return a empty string when context doesn't have columns to show", () => {
      contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const contextInterpreted = fieldsToShowExpressionObject.interpret(contextObject);

      expect(contextInterpreted).toBe('');
    });
  });
});

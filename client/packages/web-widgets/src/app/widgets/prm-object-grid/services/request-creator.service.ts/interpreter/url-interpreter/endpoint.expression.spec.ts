import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { EndpointExpression } from './endpoint.expression';

describe('endpoint.expression.spec.ts', () => {
  let sut: EndpointExpression;
  let contextObject: Context;
  let rowFilter: Partial<IGetRowsParams>;

  beforeEach(() => {
    sut = new EndpointExpression();
    rowFilter = {
      startRow: 0
    };
  });

  describe('interpret()', () => {
    it("should return the 'Deal' endpoint when it is selected as parameter", () => {
      contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toContain('/objects/v1/Deal?1');
    });

    it("should return the 'Opportunity' endpoint when it is selected as parameter", () => {
      contextObject = new Context(PrmObject.OPPORTUNITY, rowFilter as IGetRowsParams, []);
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toContain('/objects/v1/Opportunity?1');
    });

    it("should return the 'Sale' endpoint when it is selected as parameter", () => {
      contextObject = new Context(PrmObject.SALE, rowFilter as IGetRowsParams, []);
      const contextInterpreted = sut.interpret(contextObject);

      expect(contextInterpreted).toContain('/objects/v1/Sale?1');
    });
  });
});

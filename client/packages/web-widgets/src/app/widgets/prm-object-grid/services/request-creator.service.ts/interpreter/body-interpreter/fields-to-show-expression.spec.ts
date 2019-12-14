import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { FieldsToShowExpression } from './fields-to-show-expression';

describe('fields-to-show-expression.ts', () => {
  let componentUnderTest: FieldsToShowExpression;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new FieldsToShowExpression();
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
    it('should set the fields property in queryParams object', () => {
      componentUnderTest.interpret(contextMockup);

      expect(contextMockup.queryParams.fields).toEqual(['fieldTst01', 'fieldTst02', 'fieldTst03']);
    });
  });
});

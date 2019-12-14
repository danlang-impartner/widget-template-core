import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { BodyInterpreter } from './body-interpreter';

describe('body-interpreter.ts', () => {
  let componentUnderTest: BodyInterpreter;
  let contextMockup: Context;

  beforeEach(() => {
    componentUnderTest = new BodyInterpreter();
    const agGridParams: IGetRowsParams = {
      context: {},
      startRow: 0,
      endRow: 10,
      failCallback: jasmine.createSpy('failCallback'),
      filterModel: {},
      sortModel: [],
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
    it('should update the URL where to get the data', () => {
      const newContext = componentUnderTest.interpret(contextMockup);

      expect(newContext.requestUrl).toBe('/objects/v1/Opportunity');
    });
  });
});

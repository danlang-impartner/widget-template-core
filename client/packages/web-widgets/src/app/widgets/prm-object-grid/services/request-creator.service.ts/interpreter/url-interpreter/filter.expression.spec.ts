import { IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import { Context } from '../context';
import { FilterExpression } from './filter.expression';

describe('filter.expression.ts', () => {
  let filterExpressionObject: FilterExpression;

  beforeEach(() => {
    filterExpressionObject = new FilterExpression();
  });

  describe('interpret()', () => {
    it("should map a 'contains' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          'company.name': {
            filterType: 'string',
            filter: 'xerox',
            type: 'contains'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=company.name like '%25xerox%25'");
    });

    it("should map a 'equals' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          name: {
            filterType: 'string',
            filter: 'program 1',
            type: 'equals'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=name = 'program 1'");
    });

    it("should map a 'no contains' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          name: {
            filterType: 'string',
            filter: 'program 2',
            type: 'notContains'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=name not like '%25program 2%25'");
    });

    it("should map a 'no equal' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          'company.name': {
            filterType: 'string',
            filter: 'microsoft',
            type: 'notEqual'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=company.name != 'microsoft'");
    });

    it("should map a 'starts with' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          'company.name': {
            filterType: 'string',
            filter: 'face',
            type: 'startsWith'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=company.name like 'face%25'");
    });

    it("should map a 'ends with' filter to the corresponding REST API filter", () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          'company.name': {
            filterType: 'string',
            filter: 'face',
            type: 'endsWith'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      const interpretedFilter = filterExpressionObject.interpret(contextObject);

      expect(interpretedFilter).toBe("&filter=company.name like '%25face'");
    });

    describe("and when use a 'less than' filter", () => {
      it('should map when the data type is number', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            account: {
              filterType: 'number',
              filter: '10',
              type: 'lessThan'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe("&filter=account < '10'");
      });

      it('should map when the data type is date', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            accountCreated: {
              filterType: 'date',
              dateFrom: '2016-03-05',
              type: 'lessThan'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe("&filter=accountCreated < '2016/03/05'");
      });
    });

    describe("and when use a 'less than or equal' filter", () => {
      it('should map when the data type is number', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            account: {
              filterType: 'number',
              filter: '10',
              type: 'lessThanOrEqual'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe("&filter=account <= '10'");
      });

      it('should map when the data type is date', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            accountCreated: {
              filterType: 'date',
              dateFrom: '2016-03-05',
              type: 'lessThanOrEqual'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe("&filter=accountCreated <= '2016/03/05'");
      });
    });

    describe("and when use a 'inRange' filter", () => {
      it('should map when the data type is number', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            account: {
              filterType: 'number',
              filter: '10',
              filterTo: '20',
              type: 'inRange'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe("&filter=account >= '10' and account <= '20'");
      });

      it('should map when the data type is date', () => {
        const rowFilter: Partial<IGetRowsParams> = {
          filterModel: {
            accountCreated: {
              filterType: 'date',
              dateFrom: '2016-03-05',
              dateTo: '2016-03-30',
              type: 'inRange'
            }
          }
        };
        const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
        const interpretedFilter = filterExpressionObject.interpret(contextObject);

        expect(interpretedFilter).toBe(
          "&filter=accountCreated >= '2016/03/05' and accountCreated <= '2016/03/30'"
        );
      });
    });

    it('should throw an error when use an unrecognized operator', () => {
      const rowFilter: Partial<IGetRowsParams> = {
        filterModel: {
          accountCreated: {
            filterType: 'string',
            filter: 'google',
            type: 'have'
          }
        }
      };
      const contextObject = new Context(PrmObject.DEAL, rowFilter as IGetRowsParams, []);
      expect(() => {
        const interpretedFilter = filterExpressionObject.interpret(contextObject);
      }).toThrowError();
    });
  });
});

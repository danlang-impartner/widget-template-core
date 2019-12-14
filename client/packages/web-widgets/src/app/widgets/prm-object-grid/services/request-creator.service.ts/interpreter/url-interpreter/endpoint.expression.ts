import { Context } from '../context';
import { IExpression } from '../interfaces/expression.interface';
import { FieldsToShowExpression } from './fields-to-show.expression';
import { FilterExpression } from './filter.expression';
import { PaginationExpression } from './pagination.expression';
import { SortByExpression } from './sort-by.expression';

export class EndpointExpression implements IExpression {
  private _expressionsInterpreters: IExpression[];

  constructor() {
    this._expressionsInterpreters = [
      new FieldsToShowExpression(),
      new PaginationExpression(),
      new SortByExpression(),
      new FilterExpression()
    ];
  }

  public interpret(context: Context): string {
    let endpointUrl = '';

    switch (context.prmObject) {
      case 'Deal': {
        endpointUrl = '/objects/v1/Deal?1';
        break;
      }
      case 'Opportunity': {
        endpointUrl = '/objects/v1/Opportunity?1';
        break;
      }
      case 'Sale': {
        endpointUrl = '/objects/v1/Sale?1';
        break;
      }
      default: {
        endpointUrl = '';
        break;
      }
    }

    if (endpointUrl !== '') {
      this._expressionsInterpreters.forEach(expressionInterpreter => {
        endpointUrl += expressionInterpreter.interpret(context);
      });
    }

    return endpointUrl;
  }
}

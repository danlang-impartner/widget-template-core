import { Context } from '../context';
import { IExpression } from '../interfaces/expression.interface';
import { FieldsToShowExpression } from './fields-to-show-expression';
import { GridFilterExpression } from './grid-filter-expression';
import { PaginationExpression } from './pagination-expression';
import { SettingsRowFilterExpression } from './settings-row-filter-expression';
import { SortByExpression } from './sort-by.expression';

export class BodyInterpreter implements IExpression<Context> {
  private readonly _expressionsInterpreters: IExpression<void>[] = [
    new FieldsToShowExpression(),
    new GridFilterExpression(),
    new SettingsRowFilterExpression(),
    new SortByExpression(),
    new PaginationExpression()
  ];

  private readonly _endpointTemplate = '/objects/v1/%object%';

  public interpret(context: Context): Context {
    context.requestUrl = this._endpointTemplate.replace('%object%', context.prmObject);

    this._expressionsInterpreters.forEach(expression => expression.interpret(context));

    return context;
  }
}

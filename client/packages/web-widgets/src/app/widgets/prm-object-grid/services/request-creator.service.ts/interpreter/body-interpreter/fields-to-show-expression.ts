import { Context } from '../context';
import { IExpression } from '../interfaces/expression.interface';

export class FieldsToShowExpression implements IExpression<void> {
  public interpret(context: Context): void {
    context.queryParams.fields = [...context.fieldsNameToShow];
  }
}

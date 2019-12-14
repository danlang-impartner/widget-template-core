import { Context } from '../context';
import { IExpression } from '../interfaces';

export class FieldsToShowExpression implements IExpression {
  public interpret(context: Context): string {
    let fieldsToShowInterpretation = '';
    const fieldsNamesAsString = context.fieldsNameToShow.reduce((prev, curr): string => {
      prev = prev === '' ? curr : prev + ',' + curr;

      return prev;
    }, '');
    fieldsToShowInterpretation = fieldsNamesAsString === '' ? '' : `&fields=${fieldsNamesAsString}`;

    return fieldsToShowInterpretation;
  }
}

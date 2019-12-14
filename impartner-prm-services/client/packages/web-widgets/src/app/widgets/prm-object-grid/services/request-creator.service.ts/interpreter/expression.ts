import { Context } from './context';

export interface IExpression {
  interpret(context: Context): string;
}

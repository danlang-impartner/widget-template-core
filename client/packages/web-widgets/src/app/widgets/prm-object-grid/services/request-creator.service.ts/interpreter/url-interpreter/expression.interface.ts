import { Context } from '../context';

export interface IExpression<T = string> {
  interpret(context: Context): T;
}

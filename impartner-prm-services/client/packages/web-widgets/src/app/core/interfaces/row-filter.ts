export class IRowFilter {
  public id: number;
  public booleanOperator: 'and' | 'or';
  public fact: string;
  public operator: string;
  public value: string[];
}

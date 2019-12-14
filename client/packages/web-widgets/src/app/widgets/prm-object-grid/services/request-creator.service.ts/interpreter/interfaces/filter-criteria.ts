export interface IFilterCriteria<T = any> {
  id: number;
  criteriaType:
    | 'None'
    | 'Boolean'
    | 'Number'
    | 'Date'
    | 'Datetime'
    | 'String'
    | 'StringList'
    | 'Guid'
    | 'Url'
    | 'RVObjectName';
  fieldPath: string;
  comparisonType:
    | 'Equals'
    | 'NotEquals'
    | 'Between'
    | 'GreaterThan'
    | 'GreaterThanEqual'
    | 'LessThan'
    | 'LessThanEqual'
    | 'StartsWith'
    | 'EndsWith'
    | 'Contains'
    | 'ContainsAll'
    | 'ContainsAny'
    | 'CardinalityEquals'
    | 'CardinalityGreaterThan'
    | 'CardinalityLessThan'
    | 'WildcardMatch';
  compareValue: T;
  evaluationType: 'Value' | 'Formula';
}

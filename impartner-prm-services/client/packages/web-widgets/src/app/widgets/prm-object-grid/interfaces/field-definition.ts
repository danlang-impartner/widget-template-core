export interface IFieldDefinition {
  id: string;
  name: string;
  display: string;
  fieldType: string;
  filterCriteriaType?: string;
  fkFieldType?: string;
  fkFieldTypeDisplay?: string;
  show?: boolean;
}

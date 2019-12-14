import { IFieldDefinition } from '../interfaces';

export const fieldDefinitions: IFieldDefinition[] = [
  { id: 'DEL1', display: 'Name', name: 'Name', fieldType: 'string' },
  {
    id: 'DEL32',
    display: 'Company',
    name: 'Company',
    fieldType: 'Fk',
    fkFieldType: 'Company',
    fkFieldTypeDisplay: 'Company'
  },
  { id: 'DEL39', display: 'Source', name: 'Source', fieldType: 'StandardPicklist' },
  { id: 'DEL3', display: 'Updated', name: 'Updated', fieldType: 'DateTime' },
  { id: 'DEL41', display: 'Close Date', name: 'CloseDate', fieldType: 'DateTime' },
  { id: 'DEL14', display: 'Record Version', name: 'RecordVersion', fieldType: 'Integer' },
  { id: 'DEL40', display: 'Amount', name: 'Amount', fieldType: 'Integer' },
  { id: 'DEL37', display: 'Probability', name: 'Probability', fieldType: 'Percent' },
  { id: 'DEL33', display: 'Primary Contact', name: 'PrimaryContact', fieldType: 'Fk' },
  {
    id: 'DEL36',
    display: 'Stage',
    name: 'Stage',
    fieldType: 'Fk',
    fkFieldType: 'DealStage',
    fkFieldTypeDisplay: 'Deal Stage'
  },
  { id: 'DEL43', name: 'Accounts', display: 'Accounts', fieldType: 'RelatedList' }
];

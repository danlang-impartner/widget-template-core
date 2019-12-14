declare module '@impartner/api' {
	export namespace prm {
		export namespace crmSync {
			type SyncScheduleType = 'None' | 'Realtime' | 'Recurring' | 'Manual';

			interface ISyncRunOverview {
				id: number;
				startedAt: Date;
				syncFromDate: Date;
				syncToDate: Date;
				errors: number;
				inbound: number;
				outbound: number;
				isDone: boolean;
				scheduleType: SyncScheduleType;
			}

			type SyncStepDirection = 'I' | 'O';

			interface ISyncStepOverview {
				id: number;
				crmObjectType: string;
				prmObjectType: string;
				templateId: number;
				direction: SyncStepDirection;
				errors: number;
				inbound: number;
				outbound: number;
				isDone: boolean;
			}

			type SyncOutcome = 'Create' | 'Update' | 'Delete' | 'Error' | 'Skipped' | 'None';

			interface ISyncStepEntry {
				prmRecordId?: number;
				crmRecordId?: string;
				outcome: SyncOutcome;
				prmRecordName: string;
				prmRecordLink: string;
				errorMessage?: string;
			}

			interface ISyncJobStatus {
				recurringStatus: IRecurringStatus;
				realtimeStatus: IRealtimeStatus;
			}

			interface IRecurringStatus {
				hasErrors: boolean;
				errorMesssage?: string;
				nextRunAttempt: Date | null;
			}

			interface IRealtimeStatus {
				isEnabled: boolean;
			}

			type SyncDeleteAction ='DeleteNone' | 'DeleteFromPrm'| 'DeleteFromCrm'| 'DeleteFromBoth';

			export interface ICrmSyncConfigPatch {
				id?: number;
				isActive?: boolean;
			}

			interface ICrmSyncConfig extends ICrmSyncConfigPatch {
				name: string;

				// PRM
				objectName: string;

				// CRM
				crmObjectName: string;
				crmName: string;
				crmId: number;
				crmType: string;
				createObjectFromCrm: boolean;
				templateFields: ISyncFieldMapping[];
				crmSyncDependencies: ICrmSyncConfig[];
				crmSyncPartnerFields?: IPartnerField[];
				prmFilters?: IFilterCriteria[];
				prmFilterCustomLogic?: string;
				crmFilters?: IFilterCriteria[];
				crmFilterCustomLogic?: string;
				createOnImportFilters?: IFilterCriteria[];
				syncDeleteAction: SyncDeleteAction;
				readonly dateLastUpdated?: Date;
				readonly lastUpdatedBy?: string;
			}

			interface IPartnerField {
				id: number;
				crmFieldPath: string;
				value: string;
				isRecordType?: boolean;
			}

			interface IPartnerFieldResult {
				partnerField: IPartnerField;
				validationResult: IValidationResult;
			}

			interface ISyncConfig {
				createObjectFromCrm: boolean;
				createOnImportFilters: IFilterCriteria[];
				crmFilterCustomLogic: string | null;
				crmFilters: IFilterCriteria[];
				crmId: number;
				crmName: string;
				crmObjectName: string;
				crmSyncPartnerFields: any[];
				crmType: string;
				dateLastUpdated: Date;
				enableRealtimeImport: boolean;
				id: number;
				isActive: boolean;
				lastRealtimeImportError: string;
				lastUpdatedBy: string;
				name: string;
				objectName: string;
				prmFilterCustomLogic: string | null;
				prmFilters: IFilterCriteria[];
				syncDeleteAction: SyncDeleteAction;
				templateFields: ITemplateField[];
				success: boolean;
			}

			type SyncDirection = 1 | 2 | 3;

			interface ITemplateField {
				createOnly: boolean;
				crmFieldPath: string;
				fieldName: string;
				fieldPath: string;
				fieldType: string;
				id: number;
				isCrmKey: boolean;
				isPrmKey: boolean;
				name: string;
				objectName: string;
				ordinal: number;
				picklistMappings: IPicklistMapping[];
				syncDirection: SyncDirection;
			}

			interface IPicklistMapping {
				crmPicklistValue: string;
				id: number;
				prmPicklistValue: string;
			}

			interface ISyncFieldMapping {
				id: number;
				name: string;
				ordinal: number;
				createOnly: boolean;
				// PRM
				fieldName: string;
				fieldType: string;
				fieldPath: string;
				fkFieldType: string;
				lookupFieldName: string;
				defaultValue: string;
				objectName: string;
				isPrmKey: boolean;
				// CRM
				crmFieldPath: string;
				crmLookupFieldName: string;
				syncDirection: SyncDirection;
				isCrmKey: boolean;
				picklistMappings?: IPicklistMapping[];
			}

			interface ISyncSaveValidationResult {
				template: ISyncFieldMapping;
				validationResult?: IValidationResult;
			}

			interface IFieldValidationResult {
				fieldMapping: ISyncFieldMapping;
				validationResult: IValidationResult;
			}

			interface IValidationError {
				contextObject: ValidationContextObjectType;
				error: string;
				id: number;
			}

			interface IValidationResult {
				isValid: boolean;
				errors: IValidationError[];
			}

			type ValidationContextObjectType = 'None' | 'Template' |  'TemplateField' |  'PicklistMap' | 'PartnerField';

			interface IValidationError {
				contextObject: ValidationContextObjectType;
				error: string;
				id: number;
			}

			type CrmSyncContext = 'prm' | 'crm';
		}
	}
}

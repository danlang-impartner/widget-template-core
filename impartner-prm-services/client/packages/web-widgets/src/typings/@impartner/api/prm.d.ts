declare module '@impartner/api' {
  export namespace prm {
    type FeatureNames =
      | 'asset'
      | 'businessPlanning'
      | 'cobranding'
      | 'content'
      | 'dataAnalytics'
      | 'dealRegistration'
      | 'leadDistribution'
      | 'moduleFormBuilder'
      | 'webSettings';

    export interface IExceptionMessage {
      message: string;
      exceptionMessage: string;
      exceptionType: string;
      stackTrace?: string;
      innerException?: IExceptionMessage;
    }

    export interface IRestException {
      exceptionMessage?: string | null;
      message: string;
      originalEvent?: IRestException | IExceptionMessage;
    }

    export interface IErrorResult {
      data: { id: number; message: string };
      errors: Array<{ message: string }>;
    }

    export interface IBaseResult {
      success: boolean;
      message?: string;
    }

    export interface IResult<T> extends IBaseResult {
      data?: T;
    }

    export interface IThqlData<T> {
      count: number;
      entity: string;
      results: T[];
    }

    export interface IParams {
      fields: string;
      filter: string;
      take: string;
      orderby: string;
      [param: string]: string;
    }

    interface IFieldBase {
      name: string;
      display: string;
    }

    export interface IPrmFieldDefinition extends IFieldBase {
      id: string;
      isReadOnly: boolean;
      isCollection: boolean;
      isNullable: boolean;
      isImportable: boolean;
      isExportable: boolean;
      isFilterable: boolean;
      isSegmentable: boolean;
      fieldType: string;
      isUniqueKey: boolean;
      foreignFieldDefinitions?: IPrmFieldDefinition[];
      isRequired: boolean;
      picklistType: string;
      values?: any;
      dataType: string;
      filterCriteriaType: string;
      displayColumn: string; // TODO: Verify is still there.
      fkFieldType?: string;
    }

    export interface ILanguage {
      id: number;
      name: string;
      ordinal?: number;
      localeCode: string;
      isDefault?: boolean;
    }

    interface IApiError {
      httpStatus: number;
      code: string;
      message: string;
    }

    interface IApiResultBase {
      success: boolean;
      message?: string;
      errors?: IApiError[];
    }

    export interface IApiResult<T> extends IApiResultBase {
      data?: T;
    }

    interface IPageableSearchResult<T> {
      hasMore: boolean;
      results: T[];
    }

    type FilterEvaluationType = 'Value' | 'Formula';

    interface IFilterCriteria {
      compareValue: string;
      comparisonType: string;
      criteriaType: string;
      evaluationType: FilterEvaluationType;
      fieldPath: string;
      id: number;
    }
  }
}

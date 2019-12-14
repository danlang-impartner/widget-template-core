import { PrmObject } from 'src/app/core/enums';
import { IGridConfiguration } from '../interfaces';
import { DEFAULT_COLUMNS_BY_PRM_OBJECT } from './columns-by-prm-object.config';

export const DEFAULT_CONFIG: IGridConfiguration = {
  businessObjectName: PrmObject.DEAL,
  resultsPerPage: [25, 50, 100, 200],
  columnsToShow: DEFAULT_COLUMNS_BY_PRM_OBJECT[PrmObject.DEAL]
};

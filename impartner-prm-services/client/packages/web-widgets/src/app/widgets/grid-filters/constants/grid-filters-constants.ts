import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

import { GridFieldTypes, OperatorSymbol } from '../enums';
import { IFieldOperator, IGridFiltersConfig, ITabConfig } from '../interfaces';

export const DEFAULT_CONFIG_NEW_TAB: ITabConfig = {
  id: 1,
  name: 'Untitled',
  gridConfig: { businessObjectName: 'Opportunity' }
};

export const DEFAULT_TABS_CONFIG: IGridFiltersConfig = {
  tabs: [
    { id: 1, name: 'Approved', gridConfig: { businessObjectName: 'Opportunity' } },
    {
      id: 2,
      name: 'Pending',
      gridConfig: {
        businessObjectName: 'Opportunity',
        columnsToShow: ['name', 'endUserCompany', 'dollarValue', 'dateOfLikelyClose']
      }
    },
    { id: 3, name: 'Closed', gridConfig: { businessObjectName: 'Opportunity' } }
  ]
};

export const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollY: true,
  useBothWheelAxes: true
};

export const OPERATORS: IFieldOperator[] = [
  {
    name: 'Match',
    icon: require('@impartner/svg-icons/equals.svg'),
    symbol: OperatorSymbol.Equals
  },
  {
    name: 'NOT Match',
    icon: require('@impartner/svg-icons/not-equal.svg'),
    symbol: OperatorSymbol.NotEqual
  },
  {
    name: 'Greather than',
    icon: require('@impartner/svg-icons/greather-than.svg'),
    symbol: OperatorSymbol.GreaterThan,
    includeOnlyFieldTypes: [GridFieldTypes.Number]
  },
  {
    name: 'Less than',
    icon: require('@impartner/svg-icons/less-than.svg'),
    symbol: OperatorSymbol.LessThan,
    includeOnlyFieldTypes: [GridFieldTypes.Number]
  },
  {
    name: 'Greather than or equal',
    icon: require('@impartner/svg-icons/greather-than-equal.svg'),
    symbol: OperatorSymbol.GreaterThanOrEqual,
    includeOnlyFieldTypes: [GridFieldTypes.Number]
  },
  {
    name: 'Less than or equal',
    icon: require('@impartner/svg-icons/less-than-equal.svg'),
    symbol: OperatorSymbol.LessThanOrEqual,
    includeOnlyFieldTypes: [GridFieldTypes.Number]
  },
  {
    name: 'Contains',
    icon: require('@impartner/svg-icons/contains.svg'),
    symbol: OperatorSymbol.Contains,
    excludeFieldTypes: [
      GridFieldTypes.Boolean,
      GridFieldTypes.Date,
      GridFieldTypes.Datetime,
      GridFieldTypes.Number,
      GridFieldTypes.Guid
    ]
  },
  {
    name: 'Before',
    icon: require('@impartner/svg-icons/date-before.svg'),
    symbol: OperatorSymbol.DateBefore,
    includeOnlyFieldTypes: [GridFieldTypes.Date, GridFieldTypes.Datetime]
  },
  {
    name: 'After',
    icon: require('@impartner/svg-icons/date-after.svg'),
    symbol: OperatorSymbol.DateAfter,
    includeOnlyFieldTypes: [GridFieldTypes.Date, GridFieldTypes.Datetime]
  },
  {
    name: 'On, or before',
    icon: require('@impartner/svg-icons/date-on-before.svg'),
    symbol: OperatorSymbol.DateAfter,
    includeOnlyFieldTypes: [GridFieldTypes.Date, GridFieldTypes.Datetime]
  },
  {
    name: 'On, or after',
    icon: require('@impartner/svg-icons/date-on-after.svg'),
    symbol: OperatorSymbol.DateAfter,
    includeOnlyFieldTypes: [GridFieldTypes.Date, GridFieldTypes.Datetime]
  }
];

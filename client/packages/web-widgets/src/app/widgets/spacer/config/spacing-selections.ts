import { SpacingSize } from '../enums';
import { ISpacingOption } from '../interfaces';

export const SPACING_SELECTIONS: ISpacingOption[] = [
  { label: 'SM', space: SpacingSize.Small },
  { label: 'MD', space: SpacingSize.Medium },
  { label: 'LG', space: SpacingSize.Large },
  { label: 'XL', space: SpacingSize.ExtraLarge }
];

import { ImpartnerWidgetTypes, WidgetMode } from '../enums';
import { IWidget } from '../interfaces';

export const IMPARTNER_WIDGETS: IWidget[] = [
  {
    type: ImpartnerWidgetTypes.ImpartnerRichTextEditor,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-rich-text-editor-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-rich-text-editor-edit'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-prm-object-grid-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-prm-object-grid-edit'
      },
      {
        widgetMode: WidgetMode.SettingsPanel,
        tagId: 'w-impartner-prm-object-grid-settings'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerDynamicForm,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-dynamic-form-content'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-dynamic-form-builder'
      },
      {
        widgetMode: WidgetMode.SettingsPopover,
        tagId: 'w-impartner-dynamic-form-settings-popover'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerLegacyHtmlEditor,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-legacy-html-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-legacy-html-edit'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerHtmlEditor,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-html-editor-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-html-editor-edit'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerApplicantLookup,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-applicant-lookup-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-applicant-lookup-edit'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerSpacer,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-spacer-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-spacer-edit'
      },
      {
        widgetMode: WidgetMode.SettingsPopover,
        tagId: 'w-impartner-spacer-settings-popover'
      }
    ]
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerGridFilters,
    widgetComponents: [
      {
        widgetMode: WidgetMode.View,
        tagId: 'w-impartner-grid-filters-view'
      },
      {
        widgetMode: WidgetMode.Edit,
        tagId: 'w-impartner-grid-filters-edit'
      },
      {
        widgetMode: WidgetMode.SettingsPanel,
        tagId: 'w-impartner-grid-filters-settings'
      }
    ]
  }
];

import { ImpartnerWidgetTypes, WidgetMode } from '@impartner/widget-runtime';

import { IWidgetRegistration } from './interfaces';
import { WidgetTag } from './widget-tag';

export const WIDGETS: IWidgetRegistration[] = [
  {
    type: ImpartnerWidgetTypes.ImpartnerRichTextEditor,
    modes: [WidgetMode.Edit],
    path: 'rich-text-editor-edit.module#RichTextEditorEditModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerRichTextEditor,
    modes: [WidgetMode.View],
    path: 'rich-text-editor-view.module#RichTextEditorViewModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerHtmlEditor,
    modes: [WidgetMode.Edit],
    path: 'html-editor.module#HtmlEditorModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerHtmlEditor,
    modes: [WidgetMode.View],
    path: 'rich-text-editor-view.module#RichTextEditorViewModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
    modes: [WidgetMode.Edit, WidgetMode.View, WidgetMode.SettingsPanel],
    path: 'prm-object-grid.module#PrmObjectGridModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerSpacer,
    modes: [WidgetMode.Edit, WidgetMode.View, WidgetMode.SettingsPopover],
    path: 'spacer.module#SpacerModule'
  },
  {
    type: ImpartnerWidgetTypes.ImpartnerGridFilters,
    modes: [WidgetMode.Edit, WidgetMode.View, WidgetMode.SettingsPanel],
    path: 'grid-filters.module#GridFiltersModule'
  }
];

export const MODULE_WIDGETS = [
  {
    module: 'GridModule',
    path: 'grid.module#GridModule',
    tags: [WidgetTag.Grid]
  },
  {
    module: 'PrmObjectGridModule',
    path: 'prm-object-grid.module#PrmObjectGridModule',
    tags: [
      WidgetTag.PrmObjectGridEdit,
      WidgetTag.PrmObjectGridView,
      WidgetTag.PrmObjectGridSettings
    ]
  },
  {
    module: 'RichTextEditorModule',
    path: 'rich-text-editor-edit.module#RichTextEditorEditModule',
    tags: [WidgetTag.RichTextEditorEdit]
  },
  {
    module: 'RichTextEditorViewModule',
    path: 'rich-text-editor-view.module#RichTextEditorViewModule',
    tags: [WidgetTag.RichTextEditorView, WidgetTag.HtmlEditorView]
  },
  {
    module: 'HtmlEditorModule',
    path: 'html-editor.module#HtmlEditorModule',
    tags: [WidgetTag.HtmlEditorEdit]
  },
  {
    module: 'HelloWorldModule',
    path: 'hello-world.module#HelloWorldModule',
    tags: [WidgetTag.HelloWorld]
  },
  {
    module: 'HelloCustomerModule',
    path: 'hello-customer.module#HelloCustomerModule',
    tags: [WidgetTag.HelloCustomer]
  },
  {
    module: 'SpacerModule',
    path: 'spacer.module#SpacerModule',
    tags: [WidgetTag.SpacerEdit, WidgetTag.SpacerView, WidgetTag.SpacerSettings]
  },
  {
    module: 'GridFiltersModule',
    path: 'grid-filters.module#GridFiltersModule',
    tags: [WidgetTag.GridFiltersEdit, WidgetTag.GridFiltersView, WidgetTag.GridFiltersSettings]
  }
];

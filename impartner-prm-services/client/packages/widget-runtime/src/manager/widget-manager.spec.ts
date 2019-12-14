import { WidgetMode } from './enums';
import { IWidget, IWidgetMetadata } from './interfaces';
import { WidgetManager } from './widget-manager';

describe('widget-manager.ts', () => {
  let widgetManager: WidgetManager;

  beforeEach(() => {
    widgetManager = new WidgetManager();
  });

  describe('addWidgetLoader()', () => {
    it('should add a widget loader', async () => {
      const loadComponentSpy: jasmine.Spy = jasmine.createSpy('loadComponentSpy');
      const widgetDefinition: IWidgetMetadata = { type: 'PrmObjectGrid' };

      widgetManager.addWidgetLoader({
        type: 'PrmObjectGrid',
        modes: [WidgetMode.View],
        loadComponent: loadComponentSpy
      });

      await widgetManager.loadComponent(widgetDefinition, WidgetMode.View);

      expect(loadComponentSpy).toHaveBeenCalled();
    });
  });

  describe('get widgets()', () => {
    it('should return the list of loaded widgets', () => {
      expect(widgetManager.widgets.map(widget => widget.type)).toContain('ImpartnerPrmObjectGrid');
    });
  });

  describe('getTag()', () => {
    it('should return a valid tag', () => {
      const tag = widgetManager.getTag(
        {
          type: 'ImpartnerRichTextEditor'
        },
        WidgetMode.Edit
      );

      expect(tag).toBe('w-impartner-rich-text-editor-edit');
    });
  });

  describe('loadComponent()', () => {
    it('should log warn when there is not a widget loader', async done => {
      spyOn(console, 'warn');
      const widgetDefinition: IWidgetMetadata = { type: 'FakeGrid' };

      await widgetManager.loadComponent(widgetDefinition, WidgetMode.View);

      expect(console.warn).toHaveBeenCalledWith(jasmine.any(String));
      done();
    });
  });

  describe('registerWidget()', () => {
    it('registers widget just once', () => {
      const widget: IWidget = {
        type: 'custom.impartner.the-widget',
        widgetComponents: [],
        widgetIcons: [],
        mainIconName: '' + 'the icon'
      };

      expect(widgetManager.widgets.length).toBe(7);

      widgetManager.registerWidget(widget);
      widgetManager.registerWidget(widget);

      expect(widgetManager.widgets.length).toBe(8);
    });

    it('does not add widget loader when source strategy is not provided', () => {
      spyOn(widgetManager, 'addWidgetLoader');

      const widget: IWidget = {
        type: 'custom.impartner.the-widget',
        widgetComponents: [],
        widgetIcons: [],
        mainIconName: '' + 'the icon'
      };

      widgetManager.registerWidget(widget);
      expect(widgetManager.addWidgetLoader).not.toHaveBeenCalled();
    });

    it('adds widget loader when loadSourceStrategy is provided', () => {
      spyOn(widgetManager, 'addWidgetLoader');

      const widget: IWidget = {
        type: 'custom.impartner.the-widget',
        widgetComponents: [
          { widgetMode: WidgetMode.View, sourceUrl: 'view.js', tagId: '' },
          { widgetMode: WidgetMode.Edit, sourceUrl: 'edit.js', tagId: '' }
        ],
        widgetIcons: [],
        mainIconName: 'the icon'
      };
      const loadSourceStrategy = (): Promise<void> => Promise.resolve();

      widgetManager.registerWidget(widget, loadSourceStrategy);

      expect((widgetManager.addWidgetLoader as jasmine.Spy).calls.argsFor(0)).toEqual([
        {
          ...widget,
          modes: ['View'],
          sourceUrl: 'view.js',
          loadComponent: jasmine.any(Function)
        }
      ]);

      expect((widgetManager.addWidgetLoader as jasmine.Spy).calls.argsFor(1)).toEqual([
        {
          ...widget,
          modes: ['Edit'],
          sourceUrl: 'edit.js',
          loadComponent: jasmine.any(Function)
        }
      ]);
    });
  });
});

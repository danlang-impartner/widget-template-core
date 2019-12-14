import { IMPARTNER_WIDGETS } from './config';
import { WidgetMode } from './enums';
import { IResourceUrl, IWidget, IWidgetLoader, IWidgetMetadata } from './interfaces';

export class WidgetManager {
  private readonly _widgets: IWidget[] = [...IMPARTNER_WIDGETS];
  private readonly _widgetLoaders: IWidgetLoader[] = [];
  private readonly _resourceUrl: IResourceUrl = {};

  public get widgets(): IWidget[] {
    return this._widgets;
  }

  public addWidgetLoader(loader: IWidgetLoader): void {
    this._widgetLoaders.push(loader);
  }

  public getTag(widget: IWidgetMetadata, mode: WidgetMode): string {
    const widgetFound = this._widgets.find(currentWidget => currentWidget.type === widget.type);

    if (!widgetFound) {
      return '';
    }

    const componentFound = widgetFound.widgetComponents.find(
      component => component.widgetMode === mode
    );

    if (!componentFound) {
      return '';
    }

    return componentFound.tagId;
  }

  public async loadComponent(widget: IWidgetMetadata, mode: WidgetMode): Promise<void> {
    const widgetLoaderFound = this._widgetLoaders.find(
      widgetLoader => widgetLoader.type === widget.type && widgetLoader.modes.includes(mode)
    );

    if (widgetLoaderFound) {
      await widgetLoaderFound.loadComponent(widgetLoaderFound);
    } else {
      console.warn(`Component not found for the ${mode} mode of '${widget.type}' widget`);
    }
  }

  public registerWidget(
    widget: IWidget,
    loadSourceStrategy?: (widgetLoader: IWidgetLoader) => Promise<void>
  ): void {
    const existingWidget = this._widgets.find(
      registeredWidget => registeredWidget.type === widget.type
    );

    if (typeof existingWidget === 'undefined') {
      this._widgets.push(widget);
    }

    if (!loadSourceStrategy) {
      return;
    }

    widget.widgetComponents.forEach(widgetComponent => {
      this.addWidgetLoader({
        ...widget,
        modes: [widgetComponent.widgetMode],
        sourceUrl: widgetComponent.sourceUrl,
        loadComponent: loadSourceStrategy
      });
    });
  }

  public addResourceUrl({ type }: IWidgetMetadata, url: string): void {
    if (!this._resourceUrl.hasOwnProperty(type)) {
      this._resourceUrl[type] = url;
    }
  }

  public getResourceUrl({ type }: IWidgetMetadata): string {
    return this._resourceUrl[type];
  }
}

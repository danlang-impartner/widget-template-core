import { ComponentFactoryResolver, Injector, Type } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { Logger } from '@impartner/config-utils';

import { LazyElementFactory } from '../webcomponents/lazy-element.factory';

export function createLazyWebComponent(
  component: Type<any>,
  injector: Injector,
  htmlTag?: string
): void {
  const componentFactoryResolver: ComponentFactoryResolver = injector.get(ComponentFactoryResolver);
  const componentFactory = componentFactoryResolver.resolveComponentFactory(component);
  const selector = htmlTag ? htmlTag : componentFactory.selector;
  const selectorClass = customElements.get(selector);

  if (typeof selectorClass !== 'undefined') {
    Logger.debug('%s widget has been already loaded. Skipping...', selector);
  } else {
    const elementStrategyFactory = new LazyElementFactory(componentFactory);
    const element = createCustomElement(component, {
      injector,
      strategyFactory: elementStrategyFactory
    });

    customElements.define(selector, element);

    Logger.debug('%cCreated WebComponent: %c%s', 'color: green', 'color: black', selector);
  }
}

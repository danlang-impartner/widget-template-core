import { ComponentNgElementStrategy } from '@angular-impartner/elements';
import { ComponentFactory, Injector } from '@angular/core';
import { NgElementStrategy, NgElementStrategyFactory } from '@angular/elements';

export class LazyElementFactory implements NgElementStrategyFactory {
  constructor(private readonly _componentFactory: ComponentFactory<any>) {}

  public create(injector: Injector): NgElementStrategy {
    return new ComponentNgElementStrategy(this._componentFactory, injector);
  }
}

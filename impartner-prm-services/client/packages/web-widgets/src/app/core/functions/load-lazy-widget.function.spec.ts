import {
  ComponentFactoryResolver,
  DoBootstrap,
  Injector,
  NgModuleFactory,
  NgModuleFactoryLoader,
  NgModuleRef
} from '@angular/core';
import { async, TestBed } from '@angular/core/testing';

import { AppComponent } from '../../app.component';
import { RichTextEditorViewComponent } from '../../widgets/rich-text-editor-view/container/rich-text-editor-view.component';
import { IWidgetDependencies } from '../interfaces';
import { lazyLoadModule } from './load-lazy-widget.function';

describe('load-lazy-widget.function.ts', () => {
  let injector: jasmine.SpyObj<Injector>;
  let ngModuleRefMock: NgModuleRef<any>;
  let systemModuleLoaderMock: jasmine.SpyObj<NgModuleFactoryLoader>;
  let componentInstanceMock: DoBootstrap & IWidgetDependencies;
  let ngModuleFactoryMock: jasmine.SpyObj<NgModuleFactory<any>>;

  beforeEach(() => {
    systemModuleLoaderMock = jasmine.createSpyObj<NgModuleFactoryLoader>('NgModuleFactoryLoader', [
      'load'
    ]);

    ngModuleFactoryMock = jasmine.createSpyObj<NgModuleFactory<any>>('NgModuleFactory', ['create']);

    componentInstanceMock = {
      ngDoBootstrap: jasmine.createSpy('ngDoBootstrap')
    };

    const ngComponentFactoryResolver = jasmine.createSpyObj<ComponentFactoryResolver>(
      'ComponentFactoryResolver',
      ['resolveComponentFactory']
    );

    ngModuleRefMock = {
      instance: componentInstanceMock,
      componentFactoryResolver: ngComponentFactoryResolver,
      destroy: jasmine.createSpy('destroy', () => {}),
      injector: {
        get: jasmine.createSpy('get', () => {}).and.returnValue(new RichTextEditorViewComponent())
      },
      onDestroy: jasmine.createSpy('destroy', () => {})
    };

    ngModuleFactoryMock.create.and.returnValue(ngModuleRefMock);
    systemModuleLoaderMock.load.and.returnValue(Promise.resolve(ngModuleFactoryMock));

    injector = jasmine.createSpyObj<Injector>('InjectorSpy', ['get']);
    injector.get.and.returnValue(systemModuleLoaderMock);
  });

  describe('loadLazyModule()', () => {
    it('should lazy load an existing widget module', async () => {
      const loadedWidgets = await lazyLoadModule('test.module#TestModule', injector, []);

      expect(loadedWidgets).toContain('TestModule');
    });

    it('should lazy load modules whose depend the module to load', async () => {
      const moduleRefMockWithDependencies = {
        instance: {
          ngDoBootstrap: jasmine.createSpy('ngDoBootstrap'),
          widgetDependencies: ['GridModule']
        }
      };

      const ngModuleFactoryMockWithDependencies = {
        create: jasmine.createSpy('ngModuleFactory').and.returnValue(moduleRefMockWithDependencies)
      };

      const systemModuleLoaderMockWithDependencies = {
        load: jasmine
          .createSpy('loadSpy')
          .and.returnValue(Promise.resolve(ngModuleFactoryMockWithDependencies))
      };

      const injectorSpy = {
        get: jasmine
          .createSpy('injectorSpy')
          .and.returnValue(systemModuleLoaderMockWithDependencies)
      };
      const loadedWidgets = await lazyLoadModule('test.module#TestModule', injectorSpy, []);

      expect(loadedWidgets).toContain('GridModule');
    });
  });
});

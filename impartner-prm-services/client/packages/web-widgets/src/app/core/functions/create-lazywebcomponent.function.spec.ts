import { Injector } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule } from '@angular/platform-browser-dynamic/testing';
import { Logger, LoggerLevel } from '@impartner/config-utils';

import { HelloWorldComponent } from 'src/app/widgets/hello-world/containers/hello-world.component';
import { createLazyWebComponent } from './create-lazywebcomponent.function';

describe('create-lazywebcomponent.function.ts', () => {
  let injector: Injector;
  let fixture: ComponentFixture<HelloWorldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HelloWorldComponent]
    })
      .overrideModule(BrowserDynamicTestingModule, {
        set: {
          entryComponents: [HelloWorldComponent]
        }
      })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HelloWorldComponent);
    injector = fixture.debugElement.injector;
    fixture.detectChanges();
  });

  describe('createLazyWebComponent()', () => {
    it('should show a warn message when a widget is already lazy loaded', () => {
      Logger.loggerLevel = LoggerLevel.Debug;
      spyOn(console, 'debug');
      createLazyWebComponent(HelloWorldComponent, injector);
      createLazyWebComponent(HelloWorldComponent, injector);

      expect(console.debug).toHaveBeenCalledWith(
        '%s widget has been already loaded. Skipping...',
        'w-impartner-hello-world'
      );
    });
  });
});

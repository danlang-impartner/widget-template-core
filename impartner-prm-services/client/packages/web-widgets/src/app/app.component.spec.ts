import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppComponent } from './app.component';

describe('app.component.ts', () => {
  let fixture: ComponentFixture<AppComponent>;

  beforeAll(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule],
      declarations: [AppComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
    }).compileComponents();
    fixture = TestBed.createComponent(AppComponent);
  });

  it('should create the app', () => {
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title in a h3 tag', () => {
    const compiled = fixture.debugElement.nativeElement;
    console.log('h3 content in compiled: %s', compiled.querySelector('h3').textContent);
    expect(compiled.querySelector('h3')).toBeTruthy();
  });
});

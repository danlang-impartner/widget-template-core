import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { SafePipeModule } from 'safe-pipe';

import { HelloCustomerComponent } from './hello-customer.component';

describe('hello-customer.component.ts', () => {
  let component: HelloCustomerComponent;
  let fixture: ComponentFixture<HelloCustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HelloCustomerComponent],
      imports: [SafePipeModule],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HelloCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

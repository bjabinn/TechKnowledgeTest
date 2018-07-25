import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewevaluationComponent } from './newevaluation.component';

describe('NewevaluationComponent', () => {
  let component: NewevaluationComponent;
  let fixture: ComponentFixture<NewevaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewevaluationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewevaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

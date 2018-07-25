import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PreviousevaluationComponent } from './previousevaluation.component';

describe('PreviousevaluationComponent', () => {
  let component: PreviousevaluationComponent;
  let fixture: ComponentFixture<PreviousevaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PreviousevaluationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PreviousevaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

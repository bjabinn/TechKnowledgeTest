import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MenunewevaluationComponent } from './menunewevaluation.component';

describe('MenunewevaluationComponent', () => {
  let component: MenunewevaluationComponent;
  let fixture: ComponentFixture<MenunewevaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MenunewevaluationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MenunewevaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

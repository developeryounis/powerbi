import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PowerBiEmbeddedComponent } from './power-bi-embedded.component';

describe('PowerBiEmbeddedComponent', () => {
  let component: PowerBiEmbeddedComponent;
  let fixture: ComponentFixture<PowerBiEmbeddedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PowerBiEmbeddedComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PowerBiEmbeddedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

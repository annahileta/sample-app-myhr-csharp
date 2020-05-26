import { async, ComponentFixture, TestBed } from '@angular/core/testing'; 
import { ManagerActionsComponent } from './manager-actions.component';
import { ActionsService } from '../actions.service';

class ActionsServiceStub {
  public getUser() { }
}

describe('ManagerActionsComponent', () => {
  let component: ManagerActionsComponent;
  let fixture: ComponentFixture<ManagerActionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ManagerActionsComponent],
      providers: [
        { provide: ActionsService, useClass: ActionsServiceStub },
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManagerActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

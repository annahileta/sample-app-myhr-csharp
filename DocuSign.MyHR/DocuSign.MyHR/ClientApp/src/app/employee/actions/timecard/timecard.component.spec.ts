import { async, ComponentFixture, TestBed } from "@angular/core/testing"; 
import { TimeCardComponent } from "./timecard.component";
import { ActionsService } from "../actions.service";
import { EmployeeService } from "../../employee.service";
import { Subject } from "rxjs";

class ActionsServiceStub {
  public getUser() { }
}
class EmployeeServiceStub {
  public getUser() { }; 
  public user$: Subject<string> = new Subject<string>();
} 

describe("TimeCardComponent", () => {
  let component: TimeCardComponent;
  let fixture: ComponentFixture<TimeCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TimeCardComponent],
      providers: [
        { provide: ActionsService, useClass: ActionsServiceStub },
        { provide: EmployeeService, useClass: EmployeeServiceStub },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});

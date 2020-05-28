import { RouterTestingModule } from "@angular/router/testing";
import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { TimeCardComponent } from "./timecard.component";
import { ActionsService } from "../actions.service";
import { EmployeeService } from "../../employee.service";
import { of, Observable, BehaviorSubject } from "rxjs";
import { IUser } from "../../models/user.model";
import { ReactiveFormsModule, FormBuilder } from "@angular/forms";

class ActionsServiceStub {
  public createClickWrap() {}
}

class EmployeeServiceStub {
  user$: Observable<IUser> = new BehaviorSubject(null);
  public getUser() {}
}

describe("TimeCardComponent", () => {
  let component: TimeCardComponent;
  let fixture: ComponentFixture<TimeCardComponent>;
  let actionsService: ActionsService;
  let employeeService: EmployeeService;

  const response = {
    clickWrap: "",
    baseUrl: "",
  };
  const user: IUser = {
    id: 1,
    name: "testName",
    firstName: "testFirstName",
    lastName: "testLastName",
    hireDate: "testDate",
    email: "test@email.com",
    profileImage: "",
    profileId: "testId",
  };
  const testWorkLogs = [8, 4, 8, 5, 2, 3, 0];

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TimeCardComponent],
      providers: [
        { provide: ActionsService, useClass: ActionsServiceStub },
        { provide: EmployeeService, useClass: EmployeeServiceStub },
        FormBuilder,
      ],
      imports: [ReactiveFormsModule, RouterTestingModule],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeCardComponent);
    component = fixture.componentInstance;
    actionsService = TestBed.inject(ActionsService);
    spyOn(actionsService, "createClickWrap").and.returnValue(of(response));
    employeeService = TestBed.inject(EmployeeService);
    spyOn(employeeService, "getUser").and.stub();
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("ngOnInit", () => {
    it("should call getUser method from employee service", () => {
      // act
      component.ngOnInit();
      employeeService.user$.next(user);
      // assert
      expect(employeeService.getUser).toHaveBeenCalled();
    });
    it("should get user from employee service", () => {
      expect(component.user).toEqual(null);
      // act
      component.ngOnInit();
      employeeService.user$.next(user);
      // assert
      expect(component.user).toEqual(user);
    });
  });

  describe("sendTimeCard", () => {
    it("should call createClickWrap method from actions service with appropriate parameter", () => {
      // arrange
      component.workLogs = testWorkLogs;
      // act
      component.sendTimeCard();
      // assert
      expect(actionsService.createClickWrap).toHaveBeenCalledWith(testWorkLogs);
    });
  });

  describe("updateWorkLogs", () => {
    // arrange
    beforeEach(() => {
      component.timecardForm.setValue({
        Monday: "8",
        Tuesday: "4",
        Wednesday: "8",
        Thursday: "5",
        Friday: "2",
        Saturday: "3",
        Sunday: "0",
      });
    });
    it("should map logged working hours correctly", () => {
      // act
      component.updateWorkLogs();
      // assert
      expect(component.workLogs).toEqual(testWorkLogs);
    });
    it("should calculate total working hours correctly", () => {
      // act
      component.updateWorkLogs();
      // assert
      expect(component.total).toEqual(30);
    });
  });
});

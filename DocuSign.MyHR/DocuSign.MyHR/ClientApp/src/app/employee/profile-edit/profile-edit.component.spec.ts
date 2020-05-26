import { async, ComponentFixture, TestBed } from '@angular/core/testing'; 
import { ProfileEditComponent } from './profile-edit.component';
import { EmployeeService } from '../employee.service';
import { IUser } from '../models/user.model';
import { FormsModule } from '@angular/forms'; 
import { DatePipe } from '@angular/common';

class EmployeeServiceStub {
  public saveUser() { }
}


describe('ProfileEditComponent', () => {
  let component: ProfileEditComponent;
  let fixture: ComponentFixture<ProfileEditComponent>;
  let employeeService: EmployeeService;
  let datePipe: DatePipe;
  datePipe = new DatePipe('en-US');

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProfileEditComponent],
      imports: [FormsModule],
      providers: [
        { provide: EmployeeService, useClass: EmployeeServiceStub },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileEditComponent);
    component = fixture.componentInstance;
    employeeService = TestBed.inject(EmployeeService);
    component.user = <IUser>{
      firstName: "TestName",
      lastName: "TestLastName",
      profileImage: "image",
      profileId: "id",
      hireDate: datePipe.transform("01/01/2001", 'MM/dd/yyyy'),
      address: {}
    };
    spyOn(employeeService, "saveUser").and.stub();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe("saveUser", () => {
    it(
      "should save user with correct parameters"
    ),
      () => {
        let updatedUser = <IUser>{
          firstName: "FirstNameUpdated",
          lastName: "LastNameUpdated",
          profileImage: "image",
          profileId: "id",
          hireDate: datePipe.transform("01/01/2001", 'MM/dd/yyyy'),
          address: {}
        };
        component.user = <IUser>{
          firstName: "TestName",
          lastName: "TestLastName",
          profileImage: "image",
          profileId: "id",
          hireDate: datePipe.transform("01/01/2001", 'MM/dd/yyyy'),
          address: {}
        };
        component.saveUser(<IUser>{ firstName: "FirstNameUpdated", lastName: "LastNameUpdated" });
        expect(employeeService.saveUser).toHaveBeenCalledWith(
          updatedUser
        ); 
      }; 
  });

});


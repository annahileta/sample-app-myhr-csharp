import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { ManagerActionsComponent } from "./manager-actions.component";
import { ActionsService } from "../actions.service";
import { FormBuilder } from "@angular/forms";
import { DocumentType } from "../document-type.enum";

class ActionsServiceStub { }

describe("ManagerActionsComponent", () => {
  let component: ManagerActionsComponent;
  let fixture: ComponentFixture<ManagerActionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ManagerActionsComponent],
      providers: [
        { provide: ActionsService, useClass: ActionsServiceStub },
        FormBuilder,
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManagerActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("setDocumentType", () => {
    it("should set type correctly", () => {
      //arrange
      const type = DocumentType.I9;
      //act
      component.setDocumentType(type);
      //assert
      expect(component.type).toEqual(type);
    });
  });
});

import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { RouterTestingModule } from "@angular/router/testing";
import { ActionsComponent } from "./actions.component";
import { ActionsService } from "./actions.service";
import { TranslateModule } from "@ngx-translate/core";

class ActionsServiceStub {
  public getUser() {}
}

describe("ActionsComponent", () => {
  let component: ActionsComponent;
  let fixture: ComponentFixture<ActionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, TranslateModule.forRoot()],
      declarations: [ActionsComponent],
      providers: [{ provide: ActionsService, useClass: ActionsServiceStub }],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});

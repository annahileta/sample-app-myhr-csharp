import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { EmployeeComponent } from "./employee.component";
import { EmployeeRoutingModule } from "./employee-routing.module";
import { TranslateModule } from "@ngx-translate/core";
import { ProfileComponent } from "./profile/profile.component";
import { ActionsComponent } from "./actions/actions.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

@NgModule({
  declarations: [EmployeeComponent, ProfileComponent, ActionsComponent],
  imports: [
    CommonModule,
    EmployeeRoutingModule,
    TranslateModule.forChild(),
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class EmployeeModule {}

import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { EmployeeComponent } from "./employee.component";
import { EmployeeRoutingModule } from "./employee-routing.module";
import { TranslateModule } from "@ngx-translate/core";
import { ProfileComponent } from "./profile/profile.component";
import { ActionsComponent } from "./actions/actions.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { TimeCardComponent } from "./actions/timecard/timecard.component";
import { ManagerActionsComponent } from './actions/manager-actions/manager-actions.component';

@NgModule({
  declarations: [
    EmployeeComponent,
    ProfileComponent,
    ActionsComponent,
    TimeCardComponent,
    ManagerActionsComponent,
  ],
  imports: [
    CommonModule,
    EmployeeRoutingModule,
    TranslateModule.forChild(),
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class EmployeeModule {}

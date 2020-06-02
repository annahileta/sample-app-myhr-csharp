import { SharedModule } from './../shared/shared.module'
import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { EmployeeComponent } from './employee.component'
import { EmployeeRoutingModule } from './employee-routing.module'
import { TranslateModule } from '@ngx-translate/core'
import { ProfileComponent } from './profile/profile.component'
import { ActionsComponent } from './actions/actions.component'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { ManagerActionsComponent } from './manager-actions/manager-actions.component'
import { ProfileEditComponent } from './profile-edit/profile-edit.component'
import { TimeCardComponent } from './timecard/timecard.component'

@NgModule({
  declarations: [
    EmployeeComponent,
    ProfileComponent,
    ActionsComponent,
    ManagerActionsComponent,
    ProfileEditComponent,
    TimeCardComponent
  ],
  imports: [
    CommonModule,
    EmployeeRoutingModule,
    TranslateModule.forChild(),
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class EmployeeModule {}

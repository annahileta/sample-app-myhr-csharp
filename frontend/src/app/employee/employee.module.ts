import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeComponent } from './employee.component';
import { EmployeeRoutingModule } from './employee-routing.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [EmployeeComponent],
  imports: [CommonModule, EmployeeRoutingModule, TranslateModule.forChild()],
})
export class EmployeeModule {}

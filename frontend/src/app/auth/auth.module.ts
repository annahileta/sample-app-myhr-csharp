import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthComponent } from './auth.component';
import { AuthRoutingModule } from './auth-routing.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [AuthComponent],
  imports: [CommonModule, AuthRoutingModule, TranslateModule.forChild()],
})
export class AuthModule {}

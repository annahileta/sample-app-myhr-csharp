import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FooterComponent } from '../core/footer/footer.component';
import { HeaderComponent } from '../core/header/header.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [FooterComponent, HeaderComponent],
  imports: [CommonModule, TranslateModule],
})
export class SharedModule {}

import { TranslateModule } from "@ngx-translate/core";
import { FetchDataComponent } from "./fetch-data.component";
import { NgModule } from "@angular/core";
import { Routes } from "@angular/router";
import { FetchDataRoutingModule } from "./fetch-data-routing.module";
import { CommonModule } from "@angular/common";

const routes: Routes = [{ path: "", component: FetchDataComponent }];

@NgModule({
  declarations: [FetchDataComponent],
  imports: [CommonModule, FetchDataRoutingModule, TranslateModule.forChild()],
})
export class FetchDataModule {}

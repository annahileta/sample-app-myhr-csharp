import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { AboutComponent } from "./about/about.component";

const routes: Routes = [
  {
    path: "",
    loadChildren: () => import("./home/home.module").then((m) => m.HomeModule),
  },
  {
    path: "employee",
    loadChildren: () =>
      import("./employee/employee.module").then((m) => m.EmployeeModule),
  },
  {
    path: "about",
    component: AboutComponent,
  },
  {
    path: "fetch-data",
    loadChildren: () =>
      import("./fetch-data/fetch-data.module").then((m) => m.FetchDataModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

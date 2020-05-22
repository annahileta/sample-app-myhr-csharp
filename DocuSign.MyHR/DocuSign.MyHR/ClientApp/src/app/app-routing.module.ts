import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { AboutComponent } from "./about/about.component";
import { AuthGuard } from "./core/authentication/auth.guard";

const routes: Routes = [
  {
    path: "",
    loadChildren: () => import("./home/home.module").then((m) => m.HomeModule),
    canLoad: [AuthGuard],
  },
  {
    path: "employee",
    loadChildren: () => import("./employee/employee.module").then((m) => m.EmployeeModule),
    canLoad: [AuthGuard],
  },
  {
    path: "about",
    component: AboutComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

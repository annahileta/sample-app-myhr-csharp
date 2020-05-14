import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IUser } from "./profile/user.model";
import { BehaviorSubject } from "rxjs";

@Injectable({ providedIn: "root" })
export class EmployeeService {
  user: IUser;
  userDetails: {
    accountId: string;
    userId: string;
  };

  user$ = new BehaviorSubject<IUser>(null);

  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string
  ) {}

  saveUser(user: IUser) {
    this.http.put<any>(this.baseUrl + "api/user", user);
    this.user = user;
    this.user$.next(this.user);
  }

  getUser() {
    this.http.get<any>(this.baseUrl + "api/user").subscribe(
      (result) => {
        this.user = result;
        this.user$.next({ ...this.user });
      },
      (error) => console.error(error)
    );
  }
}

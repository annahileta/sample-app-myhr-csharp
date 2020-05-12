import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IUser } from './profile/user.model';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  user: IUser;

  user$ = new BehaviorSubject<IUser>(null);

  constructor(private http: HttpClient) {}

  saveUser(user: IUser) {
    //this.http.post<any>(`${environment.apiUrl}/save`, user);  //TODO:how we should update the user?
    console.log(user);
    this.user = user;
    this.user$.next(this.user);
  }

  getUser() {
    debugger;
    this.user = {
      name: 'UserName',
      id: 12345,
      date: new Date().toUTCString(),
      imageUrl: 'http://localhost:4200/employee/1.png',
      location: {
        address: 'Street1',
        city: 'City1',
        country: 'Country',
      },
      isAdmin: true,
    };
    this.user$.next({ ...this.user });
  }
}

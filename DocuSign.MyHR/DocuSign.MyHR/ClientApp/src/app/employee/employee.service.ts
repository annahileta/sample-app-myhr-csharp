import { Injectable, Inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject } from 'rxjs'
import { IUser } from './shared/user.model'

@Injectable({ providedIn: 'root' })
export class EmployeeService {
    user: IUser
    user$ = new BehaviorSubject<IUser>(null)

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

    saveUser(user: IUser): void {
        this.http.put<any>(this.baseUrl + 'api/user', user).subscribe(
            () => {
                this.getUser()
            },
            (error) => console.error(error)
        )
    }

    getUser(): void {
        this.http.get<any>(this.baseUrl + 'api/user').subscribe(
            (result) => {
                this.user = result
                this.user$.next({ ...this.user })
            },
            (error) => console.error(error)
        )
    }
}

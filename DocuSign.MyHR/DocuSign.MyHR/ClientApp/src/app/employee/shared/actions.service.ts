import { Injectable, Inject } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { DocumentType } from './document-type.enum'
import { IUser } from './user.model'
import { Observable } from 'rxjs'

@Injectable({ providedIn: 'root' })
export class ActionsService {
  redirectUrl: string = window.location.href.split('?')[0];

  constructor (
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {}

  sendEnvelope (type: DocumentType, user: IUser):Observable<any> {
    const body: any = {
      Type: type,
      AdditionalUser: user,
      RedirectUrl: this.redirectUrl
    }
    return this.http.post<any>(
      this.baseUrl + 'api/envelope',
      JSON.stringify(body),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        })
      }
    )
  }

  createClickWrap (worklogs: number[]):Observable<any> {
    const body: any = {
      WorkLogs: worklogs
    }
    return this.http.post<any>(
      this.baseUrl + 'api/clickwrap',
      JSON.stringify(body),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        })
      }
    )
  }
}

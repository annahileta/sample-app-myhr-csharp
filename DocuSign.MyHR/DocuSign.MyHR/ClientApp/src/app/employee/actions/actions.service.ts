import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { IUser } from "../profile/user.model";

@Injectable({ providedIn: "root" })
export class ActionsService {
  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string
  ) {}

  sendEnvelop(type: string, user: IUser, redirectUrl: string) {
    const body: any = { 
        Type: type,
        AdditionalUser: user,
        RedirectUrl: redirectUrl, 
    };
    return this.http.post<any>(
      this.baseUrl + "envelope",
      JSON.stringify(body),
      {
        headers: new HttpHeaders({
          "Content-Type": "application/json",
        }),
      }
    );
  }
}

import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { IUser } from "../profile/user.model";
import { DocumentType } from "./document-type.enum";

@Injectable({ providedIn: "root" })
export class ActionsService {
  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string
  ) {}

  sendEnvelop(type: DocumentType, user: IUser, redirectUrl: string) {
    const body: any = {
      Type: type,
      AdditionalUser: user,
      RedirectUrl: redirectUrl,
    };
    return this.http.post<any>(
      this.baseUrl + "api/envelope",
      JSON.stringify(body),
      {
        headers: new HttpHeaders({
          "Content-Type": "application/json",
        }),
      }
    );
  }
}

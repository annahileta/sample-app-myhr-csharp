import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({ providedIn: "root" })
export class ToastService {
  message$ = new BehaviorSubject<string>(null);

  constructor() {}

  setToastMessage(message: string) {
    this.message$.next(message);
  }
}

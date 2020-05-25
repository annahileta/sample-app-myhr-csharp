import { Component, OnInit } from "@angular/core";
import { ActionsService } from "../actions.service";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { DOCUMENT } from "@angular/common";
import { Renderer2, Inject } from "@angular/core";
import { getWeek, startOfWeek, endOfWeek, format } from "date-fns";
import { EmployeeService } from "../../employee.service";
import { IUser } from "../../models/user.model";

declare const window: Window & { docuSignClick: any };

@Component({
  selector: "app-timecard",
  templateUrl: "./timecard.component.html",
})
export class TimeCardComponent implements OnInit {
  period: Date;
  total = 0;
  private readonly scriptUrl =
    "//demo.docusign.net/clickapi/sdk/latest/docusign-click.js";
  private isLoadedClickWrap: boolean;

  workLogs: number[] = [];
  weekDays: string[] = [
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday",
  ];

  timecardForm: FormGroup = new FormGroup({
    Monday: new FormControl("", Validators.required),
    Tuesday: new FormControl("", Validators.required),
    Wednesday: new FormControl("", Validators.required),
    Thursday: new FormControl("", Validators.required),
    Friday: new FormControl("", Validators.required),
    Saturday: new FormControl("", Validators.required),
    Sunday: new FormControl("", Validators.required),
  });
  user: IUser;

  constructor(
    private actionServise: ActionsService,
    private renderer2: Renderer2,
    @Inject(DOCUMENT) private document: Document,
    private employeeService: EmployeeService
  ) { }

  ngOnInit(): void {
    this.employeeService.getUser();
    this.employeeService.user$.subscribe((user) => (this.user = user));
  }

  getWeekInfo() {
    var currentDay = new Date();

    const currentWeek = getWeek(currentDay);
    const firstDay = format(
      startOfWeek(currentDay, {
        weekStartsOn: 1,
      }),
      "MMM dd"
    );
    const lastDay = format(
      endOfWeek(currentDay, {
        weekStartsOn: 1,
      }),
      "MMM dd"
    );
    return `W${currentWeek} (${firstDay} - ${lastDay})`;
  }

  updateWorkLogs() {
    this.workLogs = this.weekDays.map(
      (day) => +this.timecardForm.controls[day].value
    );

    this.total = this.workLogs.reduce(
      (total: number, workLog: number) => total + workLog,
      0
    );
  }

  sendTimeCard(type: string) {
    this.actionServise.createClickWrap(this.workLogs).subscribe((payload) => {
      const clickwrap = payload.clickWrap;
      const baseUrl = payload.docuSignBaseUrl;
      this.loadClickWrap(clickwrap, baseUrl);
      if (this.isLoadedClickWrap) {
      }
    });
  }

  loadClickWrap(clickwrap: any, baseUrl: string) {
    const existingScript = document.getElementById("clickwrapscript");
    if (existingScript) {
      this.showClickWrap(clickwrap, baseUrl);
    } else {
      const textScript = this.renderer2.createElement("script");
      textScript.src = this.scriptUrl;
      textScript.id = "clickwrapscript";
      this.renderer2.appendChild(this.document.body, textScript);

      textScript.onload = () => {
        this.isLoadedClickWrap = true;
        this.showClickWrap(clickwrap, baseUrl);
      };
    }
  }

  showClickWrap(clickwrap: any, baseUrl: string) {
    window.docuSignClick.Clickwrap.render(
      {
        environment: baseUrl,
        accountId: clickwrap.accountId,
        clickwrapId: clickwrap.clickwrapId,
        clientUserId: clickwrap.accountId,
      },
      "#ds-clickwrap"
    );
  }
}

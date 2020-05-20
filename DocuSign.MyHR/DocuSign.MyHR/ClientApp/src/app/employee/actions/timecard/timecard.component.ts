import { Component, OnInit } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { WeekDay } from "@angular/common";
import { ActionsService } from "../actions.service";
import { analyzeAndValidateNgModules } from "@angular/compiler";

@Component({
  selector: "app-timecard",
  templateUrl: "./timecard.component.html",
  styleUrls: ["./timecard.component.css"],
})
export class TimeCardComponent implements OnInit {
  public weekDays = WeekDay;
  public total: number;
  public period: Date;

  timecardForm: FormGroup = new FormGroup({
    Monday: new FormControl("", Validators.required),
    Tuesday: new FormControl("", Validators.required),
    Wednesday: new FormControl("", Validators.required),
    Thursday: new FormControl("", Validators.required),
    Friday: new FormControl("", Validators.required),
    Saturday: new FormControl("", Validators.required),
    Sunday: new FormControl("", Validators.required),
  });

  constructor(private actionServise: ActionsService) {}

  ngOnInit(): void {}

  getClickWrap() {
    this.actionServise.createClickWrap([]).subscribe((payload) => {
      const clickwrap = payload;
    });
  }
}

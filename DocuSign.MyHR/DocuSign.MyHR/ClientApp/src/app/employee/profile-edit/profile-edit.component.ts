import { Component, OnInit, Input } from '@angular/core';
import { IUser } from "../models/user.model";
import * as i18nIsoCountries from 'i18n-iso-countries';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.css']
})
export class ProfileEditComponent implements OnInit {
  @Input() user: IUser;

  countries = [] as Array<any>;
  constructor() { }

  ngOnInit(): void {

    i18nIsoCountries.registerLocale(require("i18n-iso-countries/langs/en.json"));
     
    var indexedArray = i18nIsoCountries.getNames("en");
    this.countries = [];
    for (let key in indexedArray) {
      let value = indexedArray[key];
      this.countries.push({ key: key, value: value });
    }
    console.log(this.countries);
  }

}

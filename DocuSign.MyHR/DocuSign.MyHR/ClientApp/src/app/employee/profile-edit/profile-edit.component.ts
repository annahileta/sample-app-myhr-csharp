import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { IUser } from '../shared/user.model'
import * as i18nIsoCountries from 'i18n-iso-countries'
import { EmployeeService } from '../employee.service'
import { NgForm } from '@angular/forms'

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html'
})
export class ProfileEditComponent implements OnInit {
  @Input() user: IUser;
  @Output() canceled = new EventEmitter<void>();
  @Output() saved = new EventEmitter<IUser>();
  countries = [] as Array<any>;
  constructor (private employeeService: EmployeeService) {}

  ngOnInit (): void {
    i18nIsoCountries.registerLocale(
      // eslint-disable-next-line @typescript-eslint/no-var-requires
      require('i18n-iso-countries/langs/en.json')
    )

    const indexedArray = i18nIsoCountries.getNames('en')
    this.countries = []
    for (const key in indexedArray) {
      const value = indexedArray[key]
      this.countries.push({ key: key, value: value })
    }
  }

  saveUser (user: IUser) {
    this.employeeService.saveUser(user)
    this.employeeService.user$.subscribe((user) => (this.user = user))
    user.profileImage = this.user.profileImage
    user.profileId = this.user.profileId
    user.hireDate = this.user.hireDate
    this.saved.next(user)
    // todo:show message
  }

  cancel () {
    this.user = { ...this.employeeService.user }
    this.canceled.next()
  }
}

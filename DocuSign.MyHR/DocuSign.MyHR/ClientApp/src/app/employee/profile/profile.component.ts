import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { EmployeeService } from '../employee.service'
import { FormGroup } from '@angular/forms'
import { Observable } from 'rxjs'
import { IUser } from '../models/user.model'

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  @Output() editUserClicked = new EventEmitter<void>();
  @Input() user: IUser;

  ngOnInit () {}

  editProfile () {
    this.editUserClicked.next()
  }
}

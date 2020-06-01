import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { EmployeeService } from '../employee.service'
import { FormGroup } from '@angular/forms'
import { Observable } from 'rxjs'
import { IUser } from '../shared/user.model'

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {
    @Output() editUserClicked = new EventEmitter<void>()
    @Input() user: IUser

    ngOnInit() {}

    editProfile(): void {
        this.editUserClicked.next()
    }
}

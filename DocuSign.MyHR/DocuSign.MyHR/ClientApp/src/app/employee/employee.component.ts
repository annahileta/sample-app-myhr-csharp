import { of, Observable } from 'rxjs'
import { IMessage } from './shared/message.model'
import { NotificationService } from '../shared/notification/notification.service'
import { EmployeeService } from './employee.service'
import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core'
import { IUser } from './shared/user.model'
import { ActivatedRoute, Params } from '@angular/router'
import { filter, map, switchMap } from 'rxjs/operators'
import { ActionsService } from './shared/actions.service'
import { DocumentType } from './shared/document-type.enum'
import { popSavedDataFromSrorage } from './shared/storage-utils'

@Component({
    selector: 'app-employee',
    templateUrl: './employee.component.html'
})
export class EmployeeComponent implements OnInit {
    isEditUser = false
    user: IUser
    directDepositPayload
    messageBody
    @ViewChild('directDepositTemplate', { static: true }) directDepositTemplate: TemplateRef<unknown>

    constructor(
        private employeeService: EmployeeService,
        private activatedRoute: ActivatedRoute,
        private notificationService: NotificationService,
        private actionServise: ActionsService
    ) {}

    ngOnInit(): void {
        this.employeeService.getUser()
        this.employeeService.user$.subscribe((user) => (this.user = user))

        this.activatedRoute.queryParams
            .pipe(
                map((params: Params) => params.event),
                filter((event: string) => !!event && event === 'signing_complete'),
                switchMap(() => this.getNotificationMessage())
            )
            .subscribe((message: IMessage) => this.notificationService.showNotificationMessage(message))
    }

    editUser(): void {
        this.isEditUser = true
    }

    cancelEdit(): void {
        this.isEditUser = false
    }

    exitSaving(user: IUser): void {
        this.user = user
        this.isEditUser = false
    }

    private getNotificationMessage(): Observable<IMessage> {
        const documentType = popSavedDataFromSrorage('documentType')
        const header = `Notifications.SuccessMessageHeader.${documentType || 'Timecard'}`

        if (documentType === DocumentType.DirectDeposit) {
            const envelopeId = popSavedDataFromSrorage('envelopeId')
            return this.actionServise.getEnvelopeInfo(envelopeId).pipe(
                map((payload) => {
                    this.directDepositPayload = payload
                    return { header, body: this.directDepositTemplate }
                })
            )
        }
        return of({ header, body: `Notifications.SuccessMessageBody.${documentType || 'Timecard'}` })
    }
}

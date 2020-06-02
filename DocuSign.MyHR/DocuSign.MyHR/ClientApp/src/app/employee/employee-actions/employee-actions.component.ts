import { Component, OnInit } from '@angular/core'
import { ActionsService } from '../shared/actions.service'
import { DocumentType } from '../shared/document-type.enum'
import { Router } from '@angular/router'

@Component({
    selector: 'app-employee-actions',
  templateUrl: './employee-actions.component.html'
})
export class EmployeeActionsComponent implements OnInit {
    public documentType = DocumentType
    actionExecuted: boolean
    constructor(private actionServise: ActionsService, private router: Router) {}

    ngOnInit(): void {}

    sendEnvelope(type: DocumentType): void {
        this.actionExecuted = true
        this.actionServise.sendEnvelope(type, null).subscribe((payload) => {
            sessionStorage.setItem('envelopeId', payload.envelopeId)
            sessionStorage.setItem('documentType', type)
            window.location.href = payload.redirectUrl
        })
    }

    sendTimeCard(): void {
        this.actionExecuted = true
        this.router.navigate(['/employee/timecard'])
    }
}

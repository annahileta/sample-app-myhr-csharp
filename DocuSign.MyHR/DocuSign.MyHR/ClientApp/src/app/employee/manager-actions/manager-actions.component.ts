import { Component, ViewChild, ElementRef } from '@angular/core'
import {
  FormControl,
  Validators,
  FormGroup,
  AbstractControl
} from '@angular/forms'
import { ActionsService } from '../shared/actions.service'
import { DocumentType } from '../shared/document-type.enum'

@Component({
  selector: 'app-manager-actions',
  templateUrl: './manager-actions.component.html'
})
export class ManagerActionsComponent {
  @ViewChild('closeModalButton') closeModalButton: ElementRef;
  documentType = DocumentType;
  type: DocumentType;

  additionalUserForm: FormGroup = new FormGroup({
    Name: new FormControl('', Validators.required),
    Email: new FormControl('', [Validators.required, Validators.email])
  });

  constructor (private actionServise: ActionsService) {}

  setDocumentType (type: DocumentType):void {
    this.type = type
  }

  sendEnvelope ():void {
    this.actionServise
      .sendEnvelope(this.type, this.additionalUserForm.value)
      .subscribe((payload) => {
        if (payload.redirectUrl != null && payload.redirectUrl !== '') {
          window.location.href = payload.redirectUrl
        }

        sessionStorage.setItem('envelopeId', payload.envelopeId)
        this.closeModalButton.nativeElement.click()
        this.additionalUserForm.reset()
      })
  }

  isInvalid (control: AbstractControl) :boolean {
    const form = <FormGroup>control
    return form.invalid && form.touched
  }
}

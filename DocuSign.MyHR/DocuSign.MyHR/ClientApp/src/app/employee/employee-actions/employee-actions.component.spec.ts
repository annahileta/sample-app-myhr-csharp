import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { RouterTestingModule } from '@angular/router/testing'
import { EmployeeActionsComponent } from './employee-actions.component'
import { ActionsService } from '../shared/actions.service'
import { TranslateModule } from '@ngx-translate/core'

class ActionsServiceStub {
    public getUser() {}
}

describe('ActionsComponent', () => {
    let component: EmployeeActionsComponent
    let fixture: ComponentFixture<EmployeeActionsComponent>

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [RouterTestingModule, TranslateModule.forRoot()],
            declarations: [EmployeeActionsComponent],
            providers: [{ provide: ActionsService, useClass: ActionsServiceStub }]
        }).compileComponents()
    }))

    beforeEach(() => {
        fixture = TestBed.createComponent(EmployeeActionsComponent)
        component = fixture.componentInstance
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})

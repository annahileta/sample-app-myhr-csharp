import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { HeaderComponent } from './header.component'
import { AuthenticationService } from '../authentication/auth.service'

class AuthenticationServiceStub {
  public logout () {}
}

describe('HeaderComponent', () => {
  let component: HeaderComponent
  let fixture: ComponentFixture<HeaderComponent>
  let authenticationService: AuthenticationService

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HeaderComponent],
      providers: [
        { provide: AuthenticationService, useClass: AuthenticationServiceStub }
      ]
    }).compileComponents()
  }))

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderComponent)
    component = fixture.componentInstance
    authenticationService = TestBed.inject(AuthenticationService)
    spyOn(authenticationService, 'logout').and.stub()
    fixture.detectChanges()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })

  describe('logout', () => {
    it('should call logout method from authentication service', () => {
      // act
      component.logout()
      // assert
      expect(authenticationService.logout).toHaveBeenCalled()
    })
  })
})

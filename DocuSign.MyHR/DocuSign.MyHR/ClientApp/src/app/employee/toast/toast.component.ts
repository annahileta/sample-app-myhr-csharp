import { Component, OnInit } from '@angular/core'
import { ToastService } from './toast.service'
import { filter } from 'rxjs/operators'

declare const $

@Component({
    selector: 'app-toasts',
    templateUrl: './toast.component.html'
})
export class ToastComponent implements OnInit {
    message: string

    constructor(private toastService: ToastService) {}

    ngOnInit() {
        $('#toast').on('hidden.bs.toast', function () {
            this.toastService.clear()
        })
        this.toastService.message$.pipe(filter((message) => !!message)).subscribe((message) => {
            this.message = message
            $('#toast').toast({ delay: 2000 })
            $('#toast').toast('show')
            // window.location.href = window.location.href.split('?')[0];
        })
    }
}

import { Injectable } from '@angular/core';

declare let alertify: any; // we dont need to import it as we already added it in angular.json and styles.css
@Injectable({
  providedIn: 'root',
})
export class AlertifyService {
  constructor() { }

  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, function (e) {
      if (e) {
        okCallback();
      } else {
      }
    });
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }
  message(message: string) {
    alertify.message(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }
}

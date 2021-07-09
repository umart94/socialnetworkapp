//Dependency injection in Angular
/*
Dependency injection (DI), is an important application design pattern. Angular has its own DI framework, which is typically used in the design of Angular applications to increase their efficiency and modularity.

Dependencies are services or objects that a class needs to perform its function. DI is a coding pattern in which a class asks for dependencies from external sources rather than creating them itself.

In Angular, the DI framework provides declared dependencies to a class when that class is instantiated. This guide explains how DI works in Angular, and how you use it to make your apps flexible, efficient, and robust, as well as testable and maintainable.


Create an injectable service class
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HeroService {
  constructor() { }
}

In traditional server-side applications the application would check permissions on the server and return a 403 error page if the user didn’t have permissions, or perhaps redirect them to a login/register page if they were not signed up.

Note
403 is a HTTP error code. Specifically, this one means Permission Denied.
We want to have the same functionality in our client-side SPA, and with Router Guards we can.

With Router Guards we can prevent users from accessing areas that they’re not allowed to access, or, we can ask them for confirmation when leaving a certain area.



*/
import { Injectable } from '@angular/core';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { CanDeactivate } from '@angular/router';

@Injectable()
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent) {
    if (component.editForm.dirty) {
      return confirm(
        'Are You Sure You want to contiune? Any Unsaved changes will be lost'
      );
    }
    return true;
  }
}

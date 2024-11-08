import { Component } from '@angular/core';
import { SignupComponent } from '../signup/signup.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [SignupComponent],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  title = 'Task management system'
}

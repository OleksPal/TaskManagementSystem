import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  signupObj = new SignupModel();

  onRegister(){
    console.log('Registration was successful');
    console.log(this.signupObj);
  }
}

export class SignupModel {
  username: string;
  email: string;
  password: string;

  constructor(){
    this.username = "";
    this.email = "";
    this.password = "";
  }
}

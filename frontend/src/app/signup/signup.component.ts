import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, NgModel } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { NewUserDto } from '../../models/newUserDto.model';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  http = inject(HttpClient);

  signupForm = new FormGroup({
    username: new FormControl<string>(''),
    email: new FormControl<string>(''),
    password: new FormControl<string>(''),
    confirmedPassword: new FormControl<string>('')
  })

  onRegister(){
    const registerNewUserRequest = {
      username: this.signupForm.value.username,
      email: this.signupForm.value.email,
      password: this.signupForm.value.password
    }

    return this.http.post<NewUserDto>('https://localhost:7197/api/users/register', registerNewUserRequest)
    .subscribe({
      next: (value) => {
        console.log(value);
      }
    })
  }
}

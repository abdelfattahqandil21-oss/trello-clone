import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [JsonPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly data = signal<object | null>(null);

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get(`${environment.apiUrl}/WeatherForecast`).subscribe(res => {
      this.data.set(res);
    });
  }
}

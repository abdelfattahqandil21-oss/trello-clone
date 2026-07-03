import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly data = signal<WeatherForecast[] | null>(null);

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<WeatherForecast[]>(`${environment.apiUrl}/WeatherForecast`).subscribe(res => {
      this.data.set(res);
    });
  }
}

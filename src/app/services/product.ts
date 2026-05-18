import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductService {

  private baseUrl = `${environment.apiUrl}/product/api/product`;

  constructor(private http: HttpClient) {}

  getProducts() {
    return this.http.get(this.baseUrl);
  }
}
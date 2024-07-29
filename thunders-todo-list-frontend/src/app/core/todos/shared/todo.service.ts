import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { TodoAdd } from './todo-add.model';
import { Todo } from './todo.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  constructor(private http: HttpClient) { }

  list() {
    return this.http.get<Todo[]>(`${environment.apiUrl}/api/todos`)
  }

  create(model: TodoAdd) {
    return this.http.post<Todo>(`${environment.apiUrl}/api/todos`, model)
  }

  alter(id: number, model: TodoAdd) {
    return this.http.put<Todo>(`${environment.apiUrl}/api/todos/${id}`, model)
  }

  delete(id: number) {
    return this.http.delete(`${environment.apiUrl}/api/todos/${id}`)
  }

  check(id: number) {
    return this.http.patch(`${environment.apiUrl}/api/todos/${id}/check`, null)
  }

  uncheck(id: number) {
    return this.http.patch(`${environment.apiUrl}/api/todos/${id}/uncheck`, null)
  }

}

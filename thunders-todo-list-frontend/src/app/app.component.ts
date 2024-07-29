import { Component } from '@angular/core';
import { ToolbarComponent } from './core/components/toolbar/toolbar.component';
import { TodoComponent } from './core/todos/todo/todo.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ToolbarComponent, TodoComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'thunders-todo-list';
}

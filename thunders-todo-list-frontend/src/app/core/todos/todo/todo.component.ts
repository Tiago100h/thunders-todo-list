import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { fadeInDownOnEnterAnimation, fadeInRightOnEnterAnimation, fadeInUpOnEnterAnimation, fadeOutDownOnLeaveAnimation, fadeOutRightOnLeaveAnimation, fadeOutUpOnLeaveAnimation, headShakeAnimation } from 'angular-animations';
import JSConfetti from 'js-confetti';
import { DialogData } from '../../shared/dialog/dialog-data.model';
import { DialogComponent } from '../../shared/dialog/dialog.component';
import { TodoAdd } from '../shared/todo-add.model';
import { Todo } from '../shared/todo.model';
import { TodoService } from '../shared/todo.service';

@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCheckboxModule, MatIconModule],
  templateUrl: './todo.component.html',
  styleUrl: './todo.component.scss',
  animations: [
    fadeInDownOnEnterAnimation({ duration: 100, delay: 100 }),
    fadeInUpOnEnterAnimation({ duration: 100, delay: 100 }),
    fadeOutDownOnLeaveAnimation({ duration: 100 }),
    fadeOutUpOnLeaveAnimation({ duration: 100 }),
    fadeInRightOnEnterAnimation(),
    fadeOutRightOnLeaveAnimation(),
    headShakeAnimation(),
  ]
})
export class TodoComponent implements OnInit {

  @ViewChild('todoTitleInput') todoTitleInput!: ElementRef<HTMLInputElement>;
  todoAdd: TodoAdd;
  todos: Todo[];
  idTodo?: number;
  readonly dialog = inject(MatDialog);
  jsConfetti = new JSConfetti();
  animState = false;
  idTodoDeleting?: number;

  get checkedList() {
    return this.todos.filter(x => x.isCompleted);
  }

  get uncheckedList() {
    return this.todos.filter(x => !x.isCompleted);
  }

  constructor(private service: TodoService) {
    this.todoAdd = { title: '' }
    this.todos = [];
  }

  ngOnInit(): void {
    this.getTasks();
  }

  private getTasks() {
    this.service.list().subscribe(todos => {
      this.todos = todos;
    });
  }

  onSubmit() {
    if (this.todoAdd.title) {
      if (this.idTodo) {
        this.service.alter(this.idTodo, this.todoAdd).subscribe(todo => {
          this.todos.find(x => x.id == this.idTodo)!.title = todo.title;
          this.idTodo = undefined;
        })
      }
      else {
        this.service.create(this.todoAdd).subscribe(todo => {
          this.todos.push(todo);
        })
      }
      this.todoAdd.title = '';
    }
  }

  onCheckboxChange(event: MatCheckboxChange, todo: Todo) {
    if (event.checked) {
      this.service.check(todo.id).subscribe({
        next: () => {
          if (this.todos.every(x => x.isCompleted)) {
            this.jsConfetti.addConfetti({ emojis: ['⚡'], emojiSize: 30 });
          }
        },
        error: () => todo.isCompleted = !event.checked
      });
    }
    else {
      this.service.uncheck(todo.id).subscribe({
        error: () => todo.isCompleted = !event.checked
      });
    }
  }

  exitEditMode() {
    this.idTodo = undefined;
    this.todoAdd.title = '';
  }

  handleEdit(todo: Todo) {
    this.idTodo = todo.id;
    this.todoAdd = { title: todo.title }
    this.todoTitleInput.nativeElement.focus();
  }

  openDialogDelete(todo: Todo) {
    const dialogData: DialogData = {
      title: 'Delete todo',
      content: `Would you like to delete "${todo.title}"?`,
      cancelText: 'Cancel',
      acceptText: 'Delete',
    }
    
    const dialogRef = this.dialog.open(DialogComponent, {
      data: dialogData
    });
    
    this.idTodoDeleting = todo.id;
    this.animState = true;

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.delete(todo.id);
      }

      this.idTodoDeleting = undefined;
      this.animState = false;
    })
  }

  onKeydownInput(event: KeyboardEvent) {
    if (event.key == 'Escape') {
      this.todoAdd.title = '';
      this.idTodo = undefined;      
    }
  }

  private delete(id: number) {
    this.service.delete(id).subscribe(() => {
      this.todos = this.todos.filter(x => x.id !== id);
    })
  }

  animDone() {
    this.animState = !this.animState;
  }

}

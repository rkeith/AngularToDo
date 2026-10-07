import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild, signal } from '@angular/core';
import { ToDoItem } from './todo-item';

@Component({
  selector: 'app-root',
  standalone: false,
  styleUrls: ['./app.css'],
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly title = signal('angulartodo.client');

  protected readonly items = signal<ToDoItem[]>([]);
  protected readonly selectedItem = signal<ToDoItem | null>(null);

  protected readonly newItemName = signal('');
  protected readonly addAttempted = signal(false);

  protected readonly editingId = signal<number | null>(null);
  protected readonly editName = signal('');

  @ViewChild('editInput') private editInput?: ElementRef<HTMLInputElement>;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.refresh();
  }

  protected refresh() {
    this.http.get<ToDoItem[]>('/api/todo').subscribe({
      next: (result) => {
        const sorted = [...result].sort((a, b) => a.id - b.id);
        this.items.set(sorted);
        const selected = this.selectedItem();
        if (selected) {
          this.selectedItem.set(sorted.find(i => i.id === selected.id) ?? null);
        }
      },
      error: (error) => console.error(error)
    });
  }

  protected selectItem(item: ToDoItem) {
    this.selectedItem.set(item);
    this.editingId.set(null);
  }

  protected isSelected(item: ToDoItem): boolean {
    return this.selectedItem()?.id === item.id;
  }

  protected addItem() {
    this.addAttempted.set(true);
    const name = this.newItemName().trim();
    if (!name) {
      return; // invalid: text box gets red outline via [class.invalid]
    }

    this.http.post<ToDoItem>('/api/todo', { itemName: name, isCompleted: 0 }).subscribe({
      next: () => {
        this.newItemName.set('');
        this.addAttempted.set(false);
        this.refresh();
      },
      error: (error) => console.error(error)
    });
  }

  protected toggleCompleted(item: ToDoItem, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.http.put(`/api/todo/${item.id}`, {
      itemName: item.itemName,
      isCompleted: checked ? 1 : 0
    }).subscribe({
      next: () => this.refresh(),
      error: (error) => console.error(error)
    });
  }

  protected startEdit() {
    const selected = this.selectedItem();
    if (!selected) {
      return;
    }
    this.editingId.set(selected.id);
    this.editName.set(selected.itemName);
    // Wait for the input to render, then focus it and place the cursor.
    setTimeout(() => {
      const el = this.editInput?.nativeElement;
      el?.focus();
      el?.setSelectionRange(el.value.length, el.value.length);
    });
  }

  protected saveEdit(item: ToDoItem) {
    const name = this.editName().trim();

    // Cancel the edit if the text was cleared or left unchanged.
    if (!name || name === item.itemName) {
      this.editingId.set(null);
      return;
    }

    this.http.put(`/api/todo/${item.id}`, {
      itemName: name,
      isCompleted: item.isCompleted ? 1 : 0
    }).subscribe({
      next: () => {
        this.editingId.set(null);
        this.refresh();
      },
      error: (error) => console.error(error)
    });
  }

  protected deleteSelected() {
    const selected = this.selectedItem();
    if (!selected) {
      return;
    }
    this.http.delete(`/api/todo/${selected.id}`).subscribe({
      next: () => {
        this.selectedItem.set(null);
        this.refresh();
      },
      error: (error) => console.error(error)
    });
  }
}

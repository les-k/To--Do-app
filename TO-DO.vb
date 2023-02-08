import tkinter as tk
import tkinter.filedialog
import sqlite3

# Connect to SQLite database
conn = sqlite3.connect("todo.db")
cursor = conn.cursor()

# Create table if it doesn't exist
cursor.execute("""
CREATE TABLE IF NOT EXISTS tasks (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    task TEXT,
    is_important INTEGER
)
""")

class TodoApp(tk.Tk):
    def __init__(self, *args, **kwargs):
        tk.Tk.__init__(self, *args, **kwargs)

        self.title("To-Do App")

        # Create a frame to hold the task list and buttons
        frame = tk.Frame(self)
        frame.pack(side="top", fill="both", expand=True)

        # Create a scrollbar for the task list
        scrollbar = tk.Scrollbar(frame)
        scrollbar.pack(side="right", fill="y")

        # Create the task list
        self.task_list = tk.Listbox(frame, yscrollcommand=scrollbar.set)
        self.task_list.pack(side="left", fill="both", expand=True)
        scrollbar.config(command=self.task_list.yview)

        # Create the buttons
        add_task_button = tk.Button(self, text="Add Task", command=self.add_task)
        add_task_button.pack(side="left")

        complete_task_button = tk.Button(self, text="Complete Task", command=self.complete_task)
        complete_task_button.pack(side="left")

        self.refresh_task_list()

    def add_task(self):
        task = tk.filedialog.askstring("Add Task", "Enter task:")
        if task is not None:
            cursor.execute("INSERT INTO tasks (task, is_important) VALUES (?, 0)", (task,))
            conn.commit()
            self.refresh_task_list()

    def complete_task(self):
        task = self.task_list.get(self.task_list.curselection())
        if task is not None:
            cursor.execute("DELETE FROM tasks WHERE task = ?", (task,))
            conn.commit()
            self.refresh_task_list()

    def refresh_task_list(self):
        self.task_list.delete(0, tk.END)
        for task in cursor.execute("SELECT task FROM tasks"):
            self.task_list.insert(tk.END, task[0])

app = TodoApp()
app.mainloop()

# Close the database connection
conn.close()








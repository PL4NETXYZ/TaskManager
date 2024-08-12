using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace TaskManager
{
    internal class TaskItemViewModel : INotifyPropertyChanged
    {
        public List<string> Priorities { get; } = new List<string> { "Choose a priority...", "Low", "Medium", "High" };

        public List<int> usedTaskIds = [];

        private ObservableCollection<TaskItem> _tasks;
        public ObservableCollection<TaskItem> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged("Tasks");
            }
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand SaveTasksCommand { get; }

        public TaskItemViewModel()
        {
            LoadTasks();

            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask, () => SelectedTask != null);
            SaveTasksCommand = new RelayCommand(SaveTasks, () => SelectedTask != null);
        }

        //Method to determine the next free TaskID
        private int GetFreeTaskId()
        {
            usedTaskIds.Sort();

            if (usedTaskIds.Count == 0)
            {
                usedTaskIds.Add(0);
                return 0;
            }

            for (int freeTaskId = 0; freeTaskId < usedTaskIds.Count; freeTaskId++)
            {
                if (usedTaskIds[freeTaskId] != freeTaskId)
                {
                    usedTaskIds.Insert(freeTaskId, freeTaskId);
                    return freeTaskId;
                }
            }

            int nextAvailableId = usedTaskIds.Count;
            usedTaskIds.Add(nextAvailableId);
            return nextAvailableId;
        }

        private void AddTask()
        {
            TaskItem newTask = new TaskItem { TaskId = GetFreeTaskId(), TaskName = "New Task", TaskDesc = "New Description...", TaskDueDate = DateTime.Now.AddDays(3), IsCompleted = false, TaskPriority = Priorities.First().ToString() };
            Tasks.Add(newTask);
            SelectedTask = newTask;
        }

        private void DeleteTask()
        {
            usedTaskIds.Remove(SelectedTask.TaskId);
            _tasks.Remove(SelectedTask);
            SelectedTask = Tasks.FirstOrDefault();
        }

        public void SaveTasks()
        {
            string json = JsonSerializer.Serialize(Tasks);
            File.WriteAllText("tasks.json", json);
        }

        private void LoadTasks()
        {
            if (File.Exists("tasks.json"))
            {
                string json = File.ReadAllText("tasks.json");
                Tasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json);
            }
            else
            {
                Tasks = new ObservableCollection<TaskItem>();
            }
        }
    }
}

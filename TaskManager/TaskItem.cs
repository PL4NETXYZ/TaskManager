namespace TaskManager
{
    internal class TaskItem
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public DateTime TaskDueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string TaskPriority { get; set; }
    }
}

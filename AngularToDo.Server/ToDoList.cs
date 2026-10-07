using System.Collections.Concurrent;

namespace AngularToDo.Server
{
    public static class ToDoList
    {
        // Static in-memory store: lives for the lifetime of the server process
        // and is shared across all web sessions. Thread-safe via ConcurrentDictionary.
        private static readonly ConcurrentDictionary<int, ToDoItem> _items = new();

        private static int _nextId;

        public static IReadOnlyCollection<ToDoItem> GetAll() =>
            _items.Values.OrderBy(item => item.Id).ToList();

        public static ToDoItem? GetById(int id) =>
            _items.TryGetValue(id, out var item) ? item : null;

        public static ToDoItem Add(ToDoItem item)
        {
            item.Id = Interlocked.Increment(ref _nextId);
            _items[item.Id] = item;
            return item;
        }

        public static bool Update(ToDoItem item)
        {
            if (!_items.TryGetValue(item.Id, out var existing))
            {
                return false;
            }

            _items.TryUpdate(item.Id, item, existing);
            return true;
        }

        public static bool Delete(int id) =>
            _items.TryRemove(id, out _);
    }
}

using System.Text.Json.Serialization;

namespace AngularToDo.Server
{

    public class ToDoItem
    {
        public int Id { get; set; }

        public string? ItemName { get; set; }

        [JsonConverter(typeof(JsonBoolConverter))]
        public bool IsCompleted { get; set; } = false;

    }

}

using Microsoft.AspNetCore.Mvc;

namespace AngularToDo.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        // GET: api/todo
        [HttpGet]
        public ActionResult<IReadOnlyCollection<ToDoItem>> GetAll()
        {
            return Ok(ToDoList.GetAll());
        }

        // GET: api/todo/5
        [HttpGet("{id:int}")]
        public ActionResult<ToDoItem> GetById(int id)
        {
            var item = ToDoList.GetById(id);
            return item is null ? NotFound() : Ok(item);
        }

        // POST: api/todo
        // Body: { "itemName": "Clean oven", "isCompleted": 0 }
        // The ID is assigned when the item is added to the store.
        [HttpPost]
        public ActionResult<ToDoItem> Create([FromBody] ToDoItem item)
        {
            var created = ToDoList.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/todo/5
        // Body: { "itemName": "Clean oven", "isCompleted": 1 }
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ToDoItem item)
        {
            item.Id = id;
            return ToDoList.Update(item) ? NoContent() : NotFound();
        }

        // DELETE: api/todo/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return ToDoList.Delete(id) ? NoContent() : NotFound();
        }
    }
}

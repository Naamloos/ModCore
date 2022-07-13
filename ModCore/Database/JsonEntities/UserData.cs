using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Database.JsonEntities
{
    public class UserData
    {
        [JsonProperty("todo_items")]
        public List<TodoItem> TodoItems = new List<TodoItem>();
    }

    public class TodoItem
    {
        [JsonProperty("done")]
        public bool Done = false;

        [JsonProperty("item")]
        public string Item = "";
    }
}

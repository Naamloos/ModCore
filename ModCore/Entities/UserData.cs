using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Entities
{
    public class UserData
    {
        [JsonProperty("todo_items")]
        public List<TodoItem> TodoItems = new List<TodoItem>();

        [JsonProperty("usertoken")]
        public string Token = "";
    }

    public class TodoItem
    {
        [JsonProperty("done")]
        public bool Done = false;

        [JsonProperty("item")]
        public string Item = "";
    }
}

using System;
using System.Linq;

namespace ClubApp.Models
{
    public class UserSimpleModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string roles { get; set; }
        public bool ifup { get; set; }
    }
    public class UserNumberModel
    {
        public string id { get; set; }
        public string state { get; set; }
        public string relname { get; set; }
    }
    public class PageDataModel
    {
        public int code { get; set; }
        public string msg { get; set; }
        public int count { get; set; }
        public IQueryable<Object> data { get; set; }
    }
}
using System;
using System.Linq;

namespace ClubApp.Models
{
    public class PageDataModel
    {
        public int code { get; set; }
        public string msg { get; set; }
        public int count { get; set; }
        public IQueryable<Object> data { get; set; }
    }
}
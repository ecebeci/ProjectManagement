using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View(GetList());
        }

        private List<List> GetList()
        {
            return new List<List>
            {
                new List {
                        Title = "List 1",
                        Works = new List<Work>()
                },
                new List {
                        Title = "List 2",
                        Works = new List<Work>()
                 },
                new List {
                        Title = "List 3",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                },
                 new List {
                        Title = "List 4",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                 },
               new List {
                        Title = "List 5",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                }
            };
        }
    }
}

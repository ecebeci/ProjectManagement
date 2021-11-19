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

        private List<Work> GetCard()
        {
            return new List<Work>
            {
                new Work {
                        Title = "Card 1",
                        CardItems = new List<ListWorks>
                        {
                            {new ListWorks{ Title= "Mertin işi 01", OrderNumber =1, Color="red"} },
                            {new ListWorks{ Title= "Mertin işi 02", OrderNumber =2, Color="blue"} },
                            {new ListWorks{ Title= "Mertin işi 03", OrderNumber =3, Color="yellow"} }
                        }
                },
                new Work {
                        Title = "Card 2",
                        CardItems = new List<ListWorks>
                        {
                            {new ListWorks{ Title= "Ahmetin işi 01", OrderNumber =1, Color="red"} },
                        }
                },
                new Work {
                        Title = "Card 3",
                        CardItems = new List<ListWorks>()
                }
            };
        }
    }
}

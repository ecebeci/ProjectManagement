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
            return new List<Work>
            {
                new List {
                        Title = "Card 1",
                        Work = new List<Work>
                        {
                            {new Work{ Title= "Mertin işi 01", OrderNumber =1, Color="red"} },
                            {new Work{ Title= "Mertin işi 02", OrderNumber =2, Color="blue"} },
                            {new Work{ Title= "Mertin işi 03", OrderNumber =3, Color="yellow"} }
                        }
                },
                new List {
                        Title = "Card 2",
                        Work = new List<Work>
                        {
                            {new Work{ Title= "Ahmetin işi 01", OrderNumber =1, Color="red"} },
                        }
                },
                new List {
                        Title = "Card 3",
                        Work = new List<Work>()
                }
            };
        }
    }
}

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
            Random rnd = new Random();
            int randomInt = rnd.Next(999, 9999);



            return View(GetCard());
        }

        private List<Card> GetCard()
        {
            return new List<Card>
            {
                new Card {
                        Title = "Card 1",
                        CardItems = new List<CardItem>
                        {
                            {new CardItem{ Title= "Mertin işi 01", OrderNumber =1, Color="red"} },
                            {new CardItem{ Title= "Mertin işi 02", OrderNumber =2, Color="blue"} },
                            {new CardItem{ Title= "Mertin işi 03", OrderNumber =3, Color="yellow"} }
                        }
                },
                new Card {
                        Title = "Card 2",
                        CardItems = new List<CardItem>
                        {
                            {new CardItem{ Title= "Ahmetin işi 01", OrderNumber =1, Color="red"} },
                        }
                },
                new Card {
                        Title = "Card 3",
                        CardItems = new List<CardItem>()
                }
            };
        }
    }
}

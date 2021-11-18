using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Card
    {
        public string Title { get; set; }
        public List<CardItem> CardItems { get; set; }
    }
}

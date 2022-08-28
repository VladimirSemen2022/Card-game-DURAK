using Card_game_DURAK.Controller.Suits;
using System;

namespace Card_game_DURAK.Game.Cards
{
    class Card
    {
        public string Name { get; set; }       //Имя карты
        public object Suit { get; set; }        //Масть
        public int Weight { get; set; }         //Вес карты в игре

        public bool CardTrump { get; set; }     //Если козырная то true иначе false

        public Card()
        {
            object[] suit = { new Clubs(), new Dimonds(), new Heards(), new Spades() };
            string[] name = { "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
            Random rndSuit = new Random();
            Random rndName = new Random();
            Name = name[Weight = rndName.Next(name.Length)];
            Suit = suit[rndSuit.Next(suit.Length)];
            CardTrump = false;
        }

        public Card (string name, object suit, bool trump)
        {
            string[] nameList = { "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
            Name = name;
            Suit = suit;
            CardTrump = trump;
            Weight = 0;
            for (int i = 0; i < 9; i++)
            {
                if (name.ToLower().Equals(nameList[i].ToLower()))
                    Weight = i;
            }
        }

        public void Show (Card card)
        {

            Console.WriteLine("+------+");
            Console.WriteLine($" {card.Name}");
            Console.WriteLine("+      +");
            Console.WriteLine($"+  {card.Suit.ToString()}  +");
            Console.WriteLine("+------+");
        }

        public override string ToString()
        {
            return $"{Name} {Suit.ToString()}";
        }
    }
}

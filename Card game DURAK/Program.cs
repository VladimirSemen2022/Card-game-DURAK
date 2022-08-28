using Card_game_DURAK.Game.Controllers;
using System;
using System.Threading;

namespace Card_game_DURAK
{
    class Program
    {
        static void Main(string[] args)
        {
            GameController Game = new GameController();
            Game.ShowPlayingField();

            do
            {
                if (Game.HowSteps)
                {
                    do
                    {
                        Game.PlayerSteps();
                        Thread.Sleep(1500);
                        if (Game.HowSteps) Game.ComputerSteps();
                    } while (Game.HowSteps);
                }
                else
                {
                    do
                    {
                        Thread.Sleep(1500);
                        Game.ComputerSteps();
                        if (!Game.HowSteps) Game.PlayerSteps();
                    } while (!Game.HowSteps);
                }
            } while (Game.LeftComputerCards() > 0 && Game.LeftPlayerCards() > 0);
        Console.Clear();
        if (Game.LeftComputerCards() == 0 && Game.LeftPlayerCards() != 0)
            Console.WriteLine("Computer is win!");
        else if (Game.LeftComputerCards() != 0 && Game.LeftPlayerCards() == 0)
            Console.WriteLine("Player is win!");
        else
            Console.WriteLine("");
        }
    }
}

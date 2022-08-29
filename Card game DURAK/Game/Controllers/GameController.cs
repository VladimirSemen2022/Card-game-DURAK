using Card_game_DURAK.Game.Cards;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Card_game_DURAK.Game.Controllers
{
    class GameController
    {
        private List <Card> Computer { get; set; }      //Колода компьютера

        public List <Card> Player { get; set; }         //Колода игрока

        private Stack <Card> Koloda { get; set; }       //Игровая колода из котрой берутся карты

        private Stack <Card> PlayField { get; set; }    //Игровое поле, куда кладутся карты при выполнении хода

        public Queue <Card> CardTrash { get; set; }     //Карточный отбой

        public Card Trump { get; set; }                 //Козырь

        public bool HowSteps { get; set; }              //Признак, определяющий кто сейчас ходит (true - игрок, false - компьютер)



        //При активации создается игровое поле при котром создается колода из 36 карти выбирается козырь, затем из колоды
        //раздаются по 6 карт как игроку так и компьютеру и определяется тот кто должен ходить первым
        public GameController()
        {
            this.Koloda = new Stack<Card>();    //Колода из которой будут браться карты игроками после раздачи
            this.PlayField = new Stack<Card>(); //Создание зоны игрового поля
            this.CardTrash = new Queue<Card>(); //Создание зоны карточного отбоя
            int numberOfCard = 36;              //Количество карт в колоде
            Card newCard;
            Trump = new Card();            //Карта, определяющая козырь

            //Перетасовка и создание колоды карт
            bool tmp;                   //Вспомагательный признак контроля
            do
            {
                do
                {
                    tmp = false;
                    newCard = new Card();
                    foreach (var item in Koloda)
                    {
                        if (item.ToString().Equals(newCard.ToString()))
                            tmp = true;
                    }
                } while (tmp == true);
                if (numberOfCard == 36)         //Определение козыря по карте внизу колоды
                    Trump = newCard;
                if (newCard.Suit.ToString().Equals(Trump.Suit.ToString()))      //Маркировка карт козырной масти
                {
                    newCard.CardTrump = true;
                    newCard.Weight += 9;
                }
                Koloda.Push(newCard);
                //Console.WriteLine($"{koloda.Peek().ToString()}");
                numberOfCard--;
            } while (numberOfCard != 0);

            //Создание колоды карт, которые будут "на руках" у компьютера и игрока
            Computer = new List<Card>();         //Компьютер
            Player = new List<Card>();           //Игрок

            //Раздача из колоды начальных 6 шт. карт
            int botInt = 20;
            int playerInt = 20;
            for (int i = 0; i < 6; i++)
            {
                Computer.Add(Koloda.Pop());      //Раздача шести карт компьютеру
                Player.Add(Koloda.Pop());        //Раздача шести карт игроку
                if (Computer[i].CardTrump && Computer[i].Weight < botInt)
                    botInt = Computer[i].Weight;
                if (Player[i].CardTrump && Player[i].Weight < playerInt)
                    playerInt = Player[i].Weight;
            }

            Sort(Computer);
            Sort(Player);

            //Определение кто первый ходит
            if (botInt < playerInt)
            {
                HowSteps = false;
                //Console.WriteLine("Computer begins!");
            }
            else
            {
                //Console.WriteLine("Player begins!");
                HowSteps = true;
            }
        }

        public void Sort(List <Card> cards)                     //Метод сортировки карт по возрастанию их стоимости в игре
        {
            cards.Sort(delegate (Card x, Card y)
            {
                if (x.Weight == y.Weight) return 0;
                else if (x.Weight < y.Weight) return -1;
                else  return 1;
            });
        }

        public void ComputerSteps()                             //Метод при активации хода компьютера
        {
            if (Computer.Count > 0 && Player.Count > 0)
            {
                bool tmp = false;
                if (HowSteps)                                   //Если партия игрока
                {
                    for (int i = 0; i < LeftComputerCards(); i++)
                    {
                        if (CheckCard(Computer[i]))             //Проверяется каждая карта компьютера на возможность побить карту игрока
                        {
                            PlayField.Push(Computer[i]);
                            Computer.RemoveAt(i);
                            i = LeftComputerCards() + 10;
                            tmp = true;
                        }
                    }
                    if (!tmp)                                   //Если карту игрока побить невозможно то компьютер забирает все карты на столе
                    {
                        TakeCardsFromField(Computer);
                        if (Player.Count < 6 && LeftKolodaCards() > 0)
                        {
                            do
                            {
                                Player.Add(GetCardFromKoloda());
                            } while (Player.Count < 6 && LeftKolodaCards() > 0);
                        }
                    }
                }
                else                                            //Если партия компьютера
                {
                    if (PlayField.Count > 0)                    //Если на столе уже есть карты
                    {
                        if (!CardAnalis(Computer, true))        //Проверяется наличие карт в колоде компьютера с номиналом, уже лежащих на столе
                        {
                            GoToTrash();                        //Если карт нужного номинала у компьютера нет то карты со стола идут в отбой
                            HowSteps = true;                    //Ход переходит к игроку
                            if (Computer.Count < 6 && LeftKolodaCards() > 0)    //Если в колоде у игрока и компьютера карт меньше 6 то они добираются из игровой колоды (если она не пустая)
                            {
                                do
                                {
                                    Computer.Add(GetCardFromKoloda());
                                } while (Computer.Count < 6 && LeftKolodaCards() > 0);
                            }
                            if (Player.Count < 6 && LeftKolodaCards() > 0)
                            {
                                do
                                {
                                    Player.Add(GetCardFromKoloda());
                                } while (Player.Count < 6 && LeftKolodaCards() > 0);
                            }
                        }
                    }
                    else                                        //На первом ходу в партии коспьютера он ходит с карты наименьшего номинала
                    {
                        if (Computer.Count > 0)
                        {
                            PlayField.Push(Computer[0]);
                            Computer.RemoveAt(0);
                        }
                    }
                }
                Console.Clear();
                Sort(Player);
                Sort(Computer);
                ShowPlayingField();
            }
            else
            {
                if (Computer.Count == 0)
                {
                    ShowEnd("COMPUTER");
                    Environment.Exit(0);
                }
                else
                {
                    ShowEnd("PLAYER");
                    Environment.Exit(0);
                }
            }
        }

        public int PlayerInput()
        {
            string choose;
            int chooseInt;
            Console.SetCursorPosition(2, 6 + LeftPlayerCards());
            Console.WriteLine("0. Pass/Stop");
            Console.SetCursorPosition(2, 7 + LeftPlayerCards());
            Console.WriteLine("Choose a number of card");
            do
            {
                chooseInt = 100;
                Console.SetCursorPosition(0, 29);
                choose = Console.ReadLine();
                if (Int32.TryParse(choose, out chooseInt) && chooseInt >= 0 && chooseInt <= Player.Count)   //Если введено число от 0 до количества карт в колоде игрока
                {
                    if (PlayField.Count > 0 && chooseInt > 0 && !HowSteps && !CheckCard(Player[chooseInt - 1]))          //Если выбранная карта не ложится по номиналу или масти
                        chooseInt = 100;
                    else if (PlayField.Count > 0 && HowSteps && chooseInt > 0 && !CardAnalis(Player, false))      //
                    {
                        chooseInt = 100;
                    }
                }
                else if (choose == "X" || choose == "x")        //Для выхода из игры нажать на X
                {
                    chooseInt = -2;
                }
                if (chooseInt == 0 && PlayField.Count > 0 && HowSteps)      //Чтобы закончить ход игроку на своем ходу нужно нажать на 0 (ноль)
                {
                    chooseInt = -3;
                }
                else if (chooseInt == 0 && PlayField.Count == 0 && HowSteps)    //Если ходит игрок или игровое поле еще пустое то он не может выдать 0 и забрать карты или передать ход
                {
                    chooseInt = 100;
                }
            } while (chooseInt > Player.Count || chooseInt < -3);     //Пока введеное значение не будет в диапазоне от -3 до количества карт у игрока на руках
            return chooseInt;
        }

        public void PlayerSteps()                               //Метод при активации хода игрока
        {
            if (Player.Count > 0 && Computer.Count > 0)
            {
                int choose = PlayerInput();
                if (choose > 0 && choose <= Player.Count)           //Если выбран номер карты и пройдены все проверки на возможность ходить этой картой
                {
                    PlayField.Push(Player[choose - 1]);             //Карта игрока кладется на игровой стол
                    Player.RemoveAt(choose - 1);
                }
                else if (choose == 0)                               //Если в партии компьютера игрок решил забрать карты со стола
                {
                    TakeCardsFromField(Player);
                    HowSteps = false;                               //Ход опять у компьютера
                    if (Computer.Count < 6 && Koloda.Count > 0)
                    {
                        do
                        {
                            Computer.Add(GetCardFromKoloda());
                        } while (Computer.Count < 6 && Koloda.Count > 0);
                    }
                }
                else if (choose == -3)                              //Если в свое партии игрок решил передать ход компьютеру
                {
                    GoToTrash();
                    HowSteps = false;
                    if (Player.Count < 6 && Koloda.Count > 0)
                    {
                        do
                        {
                            Player.Add(GetCardFromKoloda());
                        } while (Player.Count < 6 && Koloda.Count > 0);
                    }
                    if (Computer.Count < 6 && Koloda.Count > 0)
                    {
                        do
                        {
                            Computer.Add(GetCardFromKoloda());
                        } while (Computer.Count < 6 && Koloda.Count > 0);
                    }
                }
                else if (choose == -2)                               //Если игрок решил выйти из программы
                {
                    Console.Clear();
                    Console.WriteLine("Program stopped!");
                    Environment.Exit(0);
                }
                Console.Clear();
                Sort(Player);
                Sort(Computer);
                ShowPlayingField();
            }
            else
            {
                if (Computer.Count == 0)
                {
                    ShowEnd("COMPUTER");
                    Environment.Exit(0);
                }
                else
                {
                    ShowEnd("PLAYER");
                    Environment.Exit(0);
                }
            }
        }

        public bool CheckCard(Card tmp)                         //Проверка, может ли выбраная карта побить лежащую на игровом поле
        {
            if (PlayField.Peek().Weight <= tmp.Weight && (PlayField.Peek().Suit.ToString().Equals(tmp.Suit.ToString()) || tmp.CardTrump))
                return true;
            else
                return false;
        }

        public bool CardAnalis(List <Card> tmp, bool comp)                     //Анализ имеющихся карт
        {
            Card [] newPlayField = new Card[PlayField.Count];
            Card [] newCards = new Card[tmp.Count];
            PlayField.CopyTo(newPlayField, 0);
            Array.Reverse(newPlayField);
            tmp.CopyTo(newCards, 0);
            bool check = false;
            for(int i = 0; i < newPlayField.Length; i++)
            {
                for (int j = 0; j < newCards.Length; j++)
                {
                    if (newPlayField[i].Name.ToString().Equals(newCards[j].Name.ToString()))
                    {
                        if (comp)
                        {
                            PlayField.Push(Computer[j]);
                            Computer.RemoveAt(j);
                        }
                        j = 100;
                        i = 100;
                        check = true;
                    }     
                }
            }
            return check;
        }

        public int LeftKolodaCards ()                           //Количество оставшихся карт в колоде 
        {
            return Koloda.Count;
        }

        public int LeftComputerCards()                          //Количество оставшихся на руках у компьютера карт
        {
            return Computer.Count;
        }

        public int LeftPlayerCards()                            //Количество оставшихся на руках у игрока карт
        {
            return Player.Count;
        }

        public Card GetCardFromKoloda()                         //Получить карту из колоды
        {
            return Koloda.Pop();
        }

        public void GoToTrash()                                 //Отправить все карты с игрового поля в отбой
        {
            int count = PlayField.Count;
            for (int i = 0; i < count; i++)
            {
                CardTrash.Enqueue(PlayField.Pop());
            }
            PlayField.Clear();
        }

        public void TakeCardsFromField(List <Card> cards)       //Забрать все карты с игрового поля
        {
            int count = PlayField.Count;
            for(int i = 0; i < count; i++)
            {
                cards.Add(PlayField.Pop());
            }
            PlayField.Clear();
        }

        public void ShowPlayerCards()                           //Отображение карт игрока на экране
        {
            int step = 1;
            foreach (var item in Player)
            {
                Console.SetCursorPosition(2, 4 + step);
                Console.WriteLine($"{step++}. {item.ToString()}");
            }
        }

        public void ShowComputerCards()                         //Отображение карт компьютера на экране
        {
            int x = Console.WindowWidth;
            int step = 1;
            foreach (var item in Computer)
            {
                Console.SetCursorPosition(x / 3 * 2 + 2, 4 + step);
                //Console.WriteLine($"{step++}. {item.ToString()}");
                Console.WriteLine($"{step++}. Card");
            }
        }

        public void ShowPlayingField()                        //Отобразить игровое поле на экране 
        {
            Console.Clear();
            int x = Console.WindowWidth;
            int y = Console.WindowHeight;
            string[] game = { "THE GAME <<DURAK>>", "PLAYER CARDS", "PLAYING FIELD", "COMPUTER CARDS", "X - Exit from game", "player step", "computer step",
                $"TRUMP - {Trump.Suit}", $" Cards in koloda - {Koloda.Count}", $" Cards in trash - {CardTrash.Count}", "T - Take cards from playing field" };
            Console.SetCursorPosition(x / 2 - game[0].Length / 2, 0);
            Console.WriteLine(game[0]);
            //Console.WriteLine();
            Console.WriteLine(new string('-', x));
            Console.WriteLine();
            Console.SetCursorPosition(x / 6 - game[1].Length / 2, 2);
            Console.WriteLine(game[1]);
            Console.SetCursorPosition(x / 2 - game[2].Length / 2, 2);
            Console.WriteLine(game[2]);
            Console.SetCursorPosition(x / 6 * 5 - game[3].Length / 2, 2);
            Console.WriteLine(game[3]);
            Console.SetCursorPosition(x / 12 * 5 - game[5].Length / 2, 4);
            Console.WriteLine(game[5]);
            Console.SetCursorPosition(x / 12 * 7 - game[6].Length / 2, 4);
            Console.WriteLine(game[6]);
            Console.SetCursorPosition(0, 3);
            Console.WriteLine(new string('-', x));
            Console.SetCursorPosition(0, 28);
            Console.WriteLine(new string('-', x));
            Console.SetCursorPosition(7, 29);
            Console.WriteLine(game[4]);
            //Console.SetCursorPosition(5 + game[4].Length, 29);
            //Console.WriteLine(game[10]);
            Console.SetCursorPosition(15 + game[4].Length + game[10].Length, 29);
            Console.WriteLine(game[8]);
            Console.SetCursorPosition(20 + game[4].Length + game[10].Length + game[8].Length, 29);
            Console.WriteLine(game[9]);
            for (int i = 0; i < 26; i++)
            {
                Console.SetCursorPosition(x / 3, 2 + i);
                Console.WriteLine("|");
                Console.SetCursorPosition(x / 3 * 2, 2 + i);
                Console.WriteLine("|");
            }
            Console.SetCursorPosition(x / 2 - game[7].Length / 2, 27);
            Console.WriteLine(game[7]);

            ShowPlayerCards();
            ShowComputerCards();

            if (PlayField.Count > 0)
            {
                Card[] tmp = new Card [PlayField.Count];
                PlayField.CopyTo(tmp, 0);
                Array.Reverse(tmp);
                for (int i = 0; i < PlayField.Count; i++)
                {
                    if (HowSteps)
                    {
                        if (i % 2 == 0)
                        {
                            Console.SetCursorPosition(x / 12 * 5 - tmp[i].ToString().Length / 2, 5 + i/2);
                            Console.WriteLine($"{tmp[i].ToString()}");
                        }
                        else
                        {
                            Console.SetCursorPosition(x / 12 * 7 - tmp[i].ToString().Length / 2, 5 + i/2);
                            Console.WriteLine($"{tmp[i].ToString()}");
                        }
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            Console.SetCursorPosition(x / 12 * 7 - tmp[i].ToString().Length / 2, 5 + i/2);
                            Console.WriteLine($"{tmp[i].ToString()}");
                        }
                        else
                        {
                            Console.SetCursorPosition(x / 12 * 5 - tmp[i].ToString().Length / 2, 5 + i/2);
                            Console.WriteLine($"{tmp[i].ToString()}");
                        }
                    }
                }
            }
        }

        public void ShowEnd (string name)
        {
            int x = Console.WindowWidth;
            Console.SetCursorPosition(x / 2 - (name.Length + 5) / 2-1, 18);
            Console.WriteLine(new string('-', name.Length + 9));
            Console.SetCursorPosition(x / 2 - (name.Length + 5)/2-1, 19);
            Console.WriteLine($"| {name} WON! |");
            Console.SetCursorPosition(x / 2 - (name.Length + 5) / 2 - 1, 20);
            Console.WriteLine(new string('-', name.Length + 9));
            Console.SetCursorPosition(x / 2 - 11, 22);
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }

    }
}

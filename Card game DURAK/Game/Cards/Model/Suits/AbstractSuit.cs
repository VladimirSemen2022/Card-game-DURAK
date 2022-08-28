
namespace Card_game_DURAK.Controller.Suits
{
    abstract class AbstractSuit
    {
        public string MySuit { get; private set; }
        public string MySighn { get; private set; }

        private AbstractSuit ()
        { }

        protected AbstractSuit(string suit, string sighn)
        {
            this.MySuit = suit;
            this.MySighn = sighn;
        }

        public override string ToString() 
        {
            return MySighn;
        }
    }
}

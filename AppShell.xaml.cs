namespace B2ACT2Juego_de_exploracion
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("about", typeof(About));
            Routing.RegisterRoute("Game", typeof(GamePage));
            Routing.RegisterRoute("Main", typeof(MainPage));
        }
    }
}

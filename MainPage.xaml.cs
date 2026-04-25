namespace B2ACT2Juego_de_exploracion
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
            btnContinuar.IsEnabled = File.Exists(ruta);
        }

        private async void btnNuevaPartida_Clicked(object sender, EventArgs e)
        {
            string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");

            if (File.Exists(ruta))
            {
                await Shell.Current.GoToAsync("NewGame");
            }
            else
            {
                await Shell.Current.GoToAsync("Game");
            }
        }

        private async void btnContinuar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Game");
        }

        private async void btnAcercaDe_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("about");
        }

    }

}

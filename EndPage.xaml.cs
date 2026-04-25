namespace B2ACT2Juego_de_exploracion
{
    public partial class EndPage : ContentPage
    {
        public EndPage()
        {
            InitializeComponent();
        }

        private async void btnFin_Clicked(object sender, EventArgs e)
        {
            string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
            await Shell.Current.GoToAsync("Main");
        }
    }
}
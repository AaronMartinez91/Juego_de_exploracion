using Plugin.Maui.Audio;

namespace B2ACT2Juego_de_exploracion
{
    public partial class EndPage : ContentPage
    {
        IAudioPlayer playerFinal;
        public EndPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            playerFinal = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("horror_sting.mp3"));
            playerFinal.Play();
        }

        private async void btnFin_Clicked(object sender, EventArgs e)
        {
            string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
            await Shell.Current.GoToAsync("about");
        }
    }
}
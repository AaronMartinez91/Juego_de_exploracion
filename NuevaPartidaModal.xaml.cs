namespace B2ACT2Juego_de_exploracion
{
    public partial class NuevaPartidaModal : ContentPage
    {
        public NuevaPartidaModal()
        {
            InitializeComponent();
        }

        private async void btnCancelar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void btnConfirmar_Clicked(object sender, EventArgs e)
        {
            string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
            await Shell.Current.GoToAsync("Game");
        }
    }
}
namespace B2ACT2Juego_de_exploracion;

public partial class About : ContentPage
{
	public About()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("Main");
        return true;
    }
}
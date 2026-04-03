namespace RestaurantApp;

public partial class MyInfo : ContentPage
{
    UserController userController = new UserController();

    public MyInfo()
	{
		InitializeComponent();
        this.BindingContext = AppSession.CurrentUser;
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        btnSave.IsEnabled = false;
        activInd.IsRunning = true;
        activInd.IsVisible = true;

        try
        {
            if(!string.IsNullOrEmpty(enAddress.Text) && !string.IsNullOrEmpty(enPhone.Text))
            {
                bool result = await userController.saveInfo(AppSession.CurrentUser, enAddress.Text, enPhone.Text);

                if (result)
                {
                    await DisplayAlertAsync("Éxito", "Datos actualizados en Makos Sushi", "OK");
                }
                else
                {
                    await DisplayAlertAsync("Error", "Tiempo de espera agotado", "OK");
                }
            }
            else
            {
                await DisplayAlertAsync(Title, "Por favor, complete todos los campos", "OK");
            }
        }
        finally
        {
            btnSave.IsEnabled = true;
            activInd.IsRunning = false;
            activInd.IsVisible = false;
        }
    }
}
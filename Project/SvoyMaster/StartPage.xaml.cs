namespace SvoyMaster;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
        AppData.Load();
        LoadLabel.Text = AppData.LoadMessage;
    }

    async void OnClientClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchPage());
    }

    async void OnMasterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MasterCabinetPage());
    }

    async void OnResetClicked(object sender, EventArgs e)
    {
        bool yes = await DisplayAlertAsync("Сброс", "Удалить учебные заявки и вернуть исходных мастеров?", "Сбросить", "Назад");
        if (!yes) return;
        AppData.Reset();
        LoadLabel.Text = "Демоданные сброшены";
    }
}

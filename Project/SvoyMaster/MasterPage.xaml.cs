namespace SvoyMaster;

public partial class MasterPage : ContentPage
{
    readonly Master master;
    public MasterPage(Master selectedMaster)
    {
        InitializeComponent();
        master = selectedMaster;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NameLabel.Text = master.Name;
        ServiceLabel.Text = master.Service;
        AboutLabel.Text = master.About;
        PriceLabel.Text = $"{master.Price} ₽";
        VerifyLabel.Text = master.IsVerified ? "Демо: проверка через Госуслуги пройдена" : "Демопроверка не пройдена";
        AvailableLabel.Text = master.IsAvailable ? "Принимает заявки" : "Сейчас занят";
        RequestButton.IsEnabled = master.IsAvailable;
    }

    async void OnRequestClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RequestPage(master));
    }
}

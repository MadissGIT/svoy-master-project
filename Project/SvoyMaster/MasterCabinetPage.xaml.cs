namespace SvoyMaster;

public partial class MasterCabinetPage : ContentPage
{
    bool loading;
    public MasterCabinetPage() { InitializeComponent(); }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        loading = true;
        var master = AppData.DemoMaster;
        NameLabel.Text = master.Name;
        ServiceLabel.Text = master.Service;
        AvailableSwitch.IsToggled = master.IsAvailable;
        VerifyLabel.Text = master.IsVerified ? "Демо: проверка пройдена" : "Демопроверка не пройдена";
        VerifyButton.IsEnabled = !master.IsVerified;
        OrdersList.ItemsSource = AppData.Data.Orders.Where(o => o.MasterId == master.Id)
            .OrderByDescending(o => o.Id).ToList();
        loading = false;
    }

    void OnAvailableToggled(object sender, ToggledEventArgs e)
    {
        if (loading) return;
        AppData.DemoMaster.IsAvailable = e.Value;
        AppData.Save();
    }

    async void OnVerifyClicked(object sender, EventArgs e)
    {
        bool yes = await DisplayAlertAsync("Демонстрация", "Настоящего входа в Госуслуги нет. Показать пример успешной проверки?", "Показать", "Назад");
        if (!yes) return;
        AppData.DemoMaster.IsVerified = true;
        AppData.Save();
        VerifyLabel.Text = "Демо: проверка пройдена";
        VerifyButton.IsEnabled = false;
    }

    async void OnOrderSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ServiceOrder order) return;
        OrdersList.SelectedItem = null;
        await Navigation.PushAsync(new OrderPage(order, true));
    }
}

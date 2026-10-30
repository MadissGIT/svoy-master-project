namespace SvoyMaster;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
        ServicePicker.SelectedIndex = 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateList();
    }

    void OnSearchChanged(object? sender, EventArgs e) => UpdateList();

    void UpdateList()
    {
        if (MastersList == null) return;
        string query = SearchEntry.Text?.Trim() ?? "";
        string service = ServicePicker.SelectedItem?.ToString() ?? "Все услуги";
        MastersList.ItemsSource = AppData.Data.Masters.Where(m =>
            (service == "Все услуги" || m.Service == service)
            && (!AvailableCheck.IsChecked || m.IsAvailable)
            && (m.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || m.Service.Contains(query, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    async void OnMasterSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Master master) return;
        MastersList.SelectedItem = null;
        await Navigation.PushAsync(new MasterPage(master));
    }

    async void OnOrdersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrdersPage());
    }
}

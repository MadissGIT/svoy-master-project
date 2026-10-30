namespace SvoyMaster;

public partial class OrdersPage : ContentPage
{
    public OrdersPage() { InitializeComponent(); }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        OrdersList.ItemsSource = AppData.Data.Orders.OrderByDescending(o => o.Id).ToList();
    }

    async void OnOrderSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ServiceOrder order) return;
        OrdersList.SelectedItem = null;
        await Navigation.PushAsync(new OrderPage(order, false));
    }
}

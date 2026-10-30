namespace SvoyMaster;

public partial class RequestPage : ContentPage
{
    readonly Master master;
    public RequestPage(Master selectedMaster)
    {
        InitializeComponent();
        master = selectedMaster;
        MasterLabel.Text = master.Summary;
        PriceLabel.Text = $"Итого за услугу: {master.Price} ₽";
        VisitDate.MinimumDate = DateTime.Today;
        VisitDate.MaximumDate = DateTime.Today.AddMonths(3);
        VisitDate.Date = DateTime.Today.AddDays(1);
    }

    async void OnSendClicked(object sender, EventArgs e)
    {
        string phone = new string((PhoneEntry.Text ?? "").Where(char.IsDigit).ToArray());
        DateTime visit = (VisitDate.Date ?? DateTime.Today).Date + (VisitTime.Time ?? TimeSpan.Zero);
        string error = "";
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) error += "Введите имя.\n";
        if (phone.Length != 11) error += "Телефон должен содержать 11 цифр.\n";
        if (string.IsNullOrWhiteSpace(AddressEntry.Text)) error += "Введите адрес.\n";
        if (visit <= DateTime.Now) error += "Выберите будущее время.\n";
        if (!master.IsAvailable) error += "Мастер сейчас не принимает заявки.";
        ErrorLabel.Text = error.Trim();
        ErrorLabel.IsVisible = error.Length > 0;
        if (error.Length > 0) return;

        SendButton.IsEnabled = false;
        var order = new ServiceOrder
        {
            Id = AppData.Data.NextId++, MasterId = master.Id, MasterName = master.Name,
            Service = master.Service, Price = master.Price, Customer = NameEntry.Text.Trim(),
            Phone = phone, Address = AddressEntry.Text.Trim(), Comment = CommentEditor.Text?.Trim() ?? "",
            VisitDate = visit, AutoPay = AutoPaySwitch.IsToggled
        };
        AppData.Data.Orders.Add(order);
        AppData.Save();
        await DisplayAlertAsync("Заявка отправлена", $"Заявка №{order.Id} появилась в списке. Ответ мастера пока не получен.", "Понятно");
        var ordersPage = new OrdersPage();
        Navigation.InsertPageBefore(ordersPage, this);
        await Navigation.PopAsync();
    }
}

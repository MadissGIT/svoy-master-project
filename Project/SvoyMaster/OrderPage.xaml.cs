namespace SvoyMaster;

public partial class OrderPage : ContentPage
{
    readonly ServiceOrder order;
    readonly bool isMaster;
    public OrderPage(ServiceOrder selectedOrder, bool masterMode)
    {
        InitializeComponent();
        order = selectedOrder;
        isMaster = masterMode;
        Refresh();
    }

    void Refresh()
    {
        TitleLabel.Text = $"Заявка №{order.Id}";
        DetailsLabel.Text = $"{order.Service}\nМастер: {order.MasterName}\nКлиент: {order.Customer}\n"
            + $"Телефон: {order.Phone}\nАдрес: {order.Address}\nВизит: {order.VisitDate:dd.MM.yyyy HH:mm}\n"
            + $"Стоимость: {order.Price} ₽\nКомментарий: {(order.Comment.Length == 0 ? "Нет" : order.Comment)}";
        StatusLabel.Text = order.Status;
        PaymentLabel.Text = $"{order.Payment}\nАвтоплатёж (демо): {(order.AutoPay ? "Включён" : "Выключен")}";
        AcceptButton.IsVisible = isMaster && order.Status == "Новая";
        RejectButton.IsVisible = AcceptButton.IsVisible;
        FinishButton.IsVisible = isMaster && order.Status == "Принята";
        ConfirmButton.IsVisible = !isMaster && order.Status == "Ожидает подтверждения";
        CancelButton.IsVisible = !isMaster && (order.Status == "Новая" || order.Status == "Принята");
        HintLabel.Text = order.Status == "Ожидает подтверждения"
            ? "Мастер отметил работу выполненной. Клиент должен подтвердить результат."
            : order.Status == "Новая" ? "Заявка ожидает ответа мастера." : "";
    }

    void UpdateStatus(string status)
    {
        if (!order.ChangeStatus(status)) return;
        AppData.Save();
        Refresh();
    }

    void OnAcceptClicked(object sender, EventArgs e) => UpdateStatus("Принята");
    void OnRejectClicked(object sender, EventArgs e) => UpdateStatus("Отклонена");
    void OnFinishClicked(object sender, EventArgs e) => UpdateStatus("Ожидает подтверждения");

    async void OnConfirmClicked(object sender, EventArgs e)
    {
        bool yes = await DisplayAlertAsync("Подтверждение", "Работа выполнена? При включённом автоплатеже появится учебная отметка об оплате.", "Подтвердить", "Назад");
        if (yes) UpdateStatus("Завершена");
    }

    async void OnCancelClicked(object sender, EventArgs e)
    {
        bool yes = await DisplayAlertAsync("Отмена", "Отменить эту заявку?", "Отменить заявку", "Назад");
        if (yes) UpdateStatus("Отменена");
    }

    async void OnStartClicked(object sender, EventArgs e) => await Navigation.PopToRootAsync();
}

namespace SvoyMaster;

public class ServiceOrder
{
    public int Id { get; set; }
    public int MasterId { get; set; }
    public string MasterName { get; set; } = "";
    public string Service { get; set; } = "";
    public string Customer { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Comment { get; set; } = "";
    public DateTime VisitDate { get; set; }
    public int Price { get; set; }
    public bool AutoPay { get; set; }
    public string Status { get; set; } = "Новая";
    public string Payment { get; set; } = "Без списания";
    public string Summary => $"№{Id}: {Service} — {Status}\n{VisitDate:dd.MM.yyyy HH:mm}, {Price} ₽";

    public bool ChangeStatus(string next)
    {
        bool allowed = (Status == "Новая" && (next == "Принята" || next == "Отклонена" || next == "Отменена"))
            || (Status == "Принята" && (next == "Ожидает подтверждения" || next == "Отменена"))
            || (Status == "Ожидает подтверждения" && next == "Завершена");
        if (!allowed) return false;
        Status = next;
        if (next == "Завершена")
            Payment = AutoPay ? $"Демо: оплачено {Price} ₽" : "Оплата при встрече";
        return true;
    }
}

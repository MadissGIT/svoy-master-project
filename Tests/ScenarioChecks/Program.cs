using System.Text.RegularExpressions;
using System.Xml.Linq;
using SvoyMaster;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
var document = XDocument.Load(Path.Combine(root, "Homework/MovieSurvey/MainPage.xaml"));
XNamespace x = "http://schemas.microsoft.com/winfx/2009/xaml";
var elements = document.Root!.DescendantsAndSelf()
    .Where(e => e.Name.Namespace != x && !e.Name.LocalName.Contains('.')).ToList();
var names = elements.Select(e => (string?)e.Attribute(x + "Name")).ToList();
if (names.Any(n => n == null || !Regex.IsMatch(n, "^_[A-Za-z][A-Za-z0-9_]*_$"))
    || names.Distinct().Count() != names.Count)
    throw new InvalidOperationException("Every survey UI element needs a unique _Name_.");
var markup = File.ReadAllText(Path.Combine(root, "Homework/MovieSurvey/MainPage.xaml"));
foreach (Match reference in Regex.Matches(markup, @"x:Reference\s+([A-Za-z0-9_]+)"))
    if (!names.Contains(reference.Groups[1].Value))
        throw new InvalidOperationException("Broken x:Reference: " + reference.Value);

foreach (int masterId in new[] { 1, 2, 3, 4 })
foreach (bool autoPay in new[] { false, true })
{
    var order = new ServiceOrder { MasterId = masterId, Price = 1500, AutoPay = autoPay };
    if (order.ChangeStatus("Завершена") || !order.ChangeStatus("Принята")
        || !order.ChangeStatus("Ожидает подтверждения") || !order.ChangeStatus("Завершена")
        || order.ChangeStatus("Отменена")
        || order.Payment != (autoPay ? "Демо: оплачено 1500 ₽" : "Оплата при встрече"))
        throw new InvalidOperationException("Completion or payment scenario failed.");
}
foreach (string terminal in new[] { "Отменена", "Отклонена" })
{
    var order = new ServiceOrder { AutoPay = true };
    if (!order.ChangeStatus(terminal) || order.ChangeStatus("Принята") || order.Payment != "Без списания")
        throw new InvalidOperationException("Cancelled/rejected orders must stay unpaid.");
}
Console.WriteLine($"PASS: {names.Count} named UI elements, valid references and order/payment scenarios.");

using System.Globalization;
using System.Text;

namespace TravelSurvey;

public partial class MainPage : ContentPage
{
    private static readonly DateTime InitialDate = new(2026, 10, 9);
    private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");
    // XAML can raise change events before all named controls exist.
    private bool _isInitializing = true;

    public MainPage()
    {
        InitializeComponent();
        _isInitializing = false;
        UpdateFormState();
    }

    private static bool TryReadBudget(string? text, out decimal budget)
    {
        var normalized = (text ?? "").Trim().Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint,
                                CultureInfo.InvariantCulture, out budget) && budget > 0;
    }

    private bool HasRequiredAnswers()
        => !string.IsNullOrWhiteSpace(_NameEntry_.Text)
           && TryReadBudget(_BudgetEntry_.Text, out _);

    private void UpdateFormState()
    {
        if (_isInitializing)
            return;

        _ReadyLabel_.IsVisible = HasRequiredAnswers();
        // A result is a snapshot: hide it after any answer changes.
        _ResultCard_.IsVisible = false;
        _ResultLabel_.Text = "";
        _ValidationLabel_.IsVisible = false;
        _ValidationLabel_.Text = "";
    }

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e) => UpdateFormState();
    private void OnEditorTextChanged(object? sender, TextChangedEventArgs e) => UpdateFormState();
    private void OnSelectionChanged(object? sender, EventArgs e) => UpdateFormState();
    private void OnRadioCheckedChanged(object? sender, CheckedChangedEventArgs e) => UpdateFormState();
    private void OnCheckChanged(object? sender, CheckedChangedEventArgs e) => UpdateFormState();
    private void OnGuideToggled(object? sender, ToggledEventArgs e) => UpdateFormState();
    private void OnDateSelected(object? sender, DateChangedEventArgs e) => UpdateFormState();
    private void OnNumberChanged(object? sender, ValueChangedEventArgs e) => UpdateFormState();

    private async void OnShowResultClicked(object? sender, EventArgs e)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(_NameEntry_.Text))
            errors.Add("Укажите ваше имя.");
        if (!TryReadBudget(_BudgetEntry_.Text, out var budget))
            errors.Add("Введите бюджет: число больше нуля.");
        var departureDate = _DepartureDate_.Date;
        if (departureDate is null)
            errors.Add("Выберите дату поездки.");

        if (errors.Count > 0)
        {
            _ResultCard_.IsVisible = false;
            _ValidationLabel_.Text = string.Join(Environment.NewLine, errors);
            _ValidationLabel_.IsVisible = true;
            await _FormScroll_.ScrollToAsync(_ValidationLabel_, ScrollToPosition.Center, true);
            return;
        }

        _ValidationLabel_.Text = "";
        _ValidationLabel_.IsVisible = false;
        var pace = new[] { _QuietRadio_, _BalancedRadio_, _ActiveRadio_ }
            .FirstOrDefault(radio => radio.IsChecked)?.Value?.ToString() ?? "Не выбран";
        var interests = new List<string>();
        if (_ExcursionsCheck_.IsChecked)
            interests.Add("Экскурсии и музеи");
        if (_FoodCheck_.IsChecked)
            interests.Add("Местная кухня");

        var summary = new StringBuilder();
        summary.AppendLine($"Путешественник: {_NameEntry_.Text.Trim()}");
        summary.AppendLine($"Бюджет: {budget.ToString("N2", RussianCulture)} ₽");
        summary.AppendLine($"Направление: {_DestinationPicker_.SelectedItem ?? "Не выбрано"}");
        summary.AppendLine($"Темп отдыха: {pace}");
        summary.AppendLine($"Дата поездки: {departureDate:dd.MM.yyyy}");
        summary.AppendLine($"Продолжительность: {_DurationStepper_.Value:0} дн.");
        summary.AppendLine($"Интересы: {(interests.Count > 0 ? string.Join(", ", interests) : "Не выбраны")}");
        summary.AppendLine($"Гид: {(_GuideSwitch_.IsToggled ? "Нужен" : "Не нужен")}");
        summary.Append($"Пожелания: {(string.IsNullOrWhiteSpace(_WishesEditor_.Text) ? "Нет" : _WishesEditor_.Text.Trim())}");
        _ResultLabel_.Text = summary.ToString();
        _ResultCard_.IsVisible = true;
        await _FormScroll_.ScrollToAsync(_ResultCard_, ScrollToPosition.Start, true);
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        _isInitializing = true;
        _NameEntry_.Text = "";
        _BudgetEntry_.Text = "";
        _WishesEditor_.Text = "";
        _DestinationPicker_.SelectedIndex = -1;
        _QuietRadio_.IsChecked = false;
        _ActiveRadio_.IsChecked = false;
        _BalancedRadio_.IsChecked = true;
        _ExcursionsCheck_.IsChecked = false;
        _FoodCheck_.IsChecked = false;
        _GuideSwitch_.IsToggled = false;
        _DepartureDate_.Date = InitialDate;
        _DurationStepper_.Value = 7;
        _isInitializing = false;
        UpdateFormState();
        await _FormScroll_.ScrollToAsync(0, 0, false);
    }
}

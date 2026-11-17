namespace MovieSurvey;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    void OnAnswerChanged(object? sender, EventArgs e)
    {
        if (_ResultLabel_ == null) return; // События бывают и при создании страницы.
        _ResultLabel_.IsVisible = false;
        _ErrorLabel_.IsVisible = false;
        _ReadyLabel_.IsVisible = !string.IsNullOrWhiteSpace(_NameEntry_.Text)
            && int.TryParse(_AgeEntry_.Text, out int age) && age >= 6 && age <= 100;
    }

    void OnShowClicked(object sender, EventArgs e)
    {
        string error = "";
        if (string.IsNullOrWhiteSpace(_NameEntry_.Text)) error += "Введите имя.\n";
        if (!int.TryParse(_AgeEntry_.Text, out int age) || age < 6 || age > 100)
            error += "Введите возраст от 6 до 100.";
        _ErrorLabel_.Text = error.Trim();
        _ErrorLabel_.IsVisible = error.Length > 0;
        _ResultLabel_.IsVisible = error.Length == 0;
        if (error.Length > 0) return;

        string place = _HomeRadio_.IsChecked ? "Дома" : _CinemaRadio_.IsChecked ? "В кинотеатре" : "По-разному";
        string genre = _GenrePicker_.SelectedItem?.ToString() ?? "Не выбран";
        string comment = string.IsNullOrWhiteSpace(_CommentEditor_.Text) ? "Нет" : _CommentEditor_.Text.Trim();
        _ResultLabel_.Text = $"Имя: {_NameEntry_.Text.Trim()}\nВозраст: {age}\nЖанр: {genre}\n"
            + $"Место: {place}\nСубтитры: {(_SubtitlesCheck_.IsChecked ? "Да" : "Нет")}\n"
            + $"Компания: {(_CompanyCheck_.IsChecked ? "Да" : "Нет")}\n"
            + $"Новинки: {(_NewsSwitch_.IsToggled ? "Да" : "Нет")}\nДата: {_WatchDate_.Date:dd.MM.yyyy}\n"
            + $"Фильмов в месяц: {_FilmsStepper_.Value:F0}\nКомментарий: {comment}";
    }

    void OnClearClicked(object sender, EventArgs e)
    {
        _NameEntry_.Text = "";
        _AgeEntry_.Text = "";
        _GenrePicker_.SelectedIndex = -1;
        _HomeRadio_.IsChecked = true;
        _CinemaRadio_.IsChecked = false;
        _BothRadio_.IsChecked = false;
        _SubtitlesCheck_.IsChecked = false;
        _CompanyCheck_.IsChecked = false;
        _NewsSwitch_.IsToggled = false;
        _WatchDate_.Date = new DateTime(2026, 10, 30);
        _FilmsStepper_.Value = 2;
        _CommentEditor_.Text = "";
        _ResultLabel_.Text = "";
        _ResultLabel_.IsVisible = false;
        _ErrorLabel_.Text = "";
        _ErrorLabel_.IsVisible = false;
        _ReadyLabel_.IsVisible = false;
    }
}

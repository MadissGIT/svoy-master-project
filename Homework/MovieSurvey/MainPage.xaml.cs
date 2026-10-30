namespace MovieSurvey;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    void OnAnswerChanged(object? sender, EventArgs e)
    {
        if (ResultLabel == null) return; // События бывают и при создании страницы.
        ResultLabel.IsVisible = false;
        ErrorLabel.IsVisible = false;
        ReadyLabel.IsVisible = !string.IsNullOrWhiteSpace(NameEntry.Text)
            && int.TryParse(AgeEntry.Text, out int age) && age >= 6 && age <= 100;
    }

    void OnShowClicked(object sender, EventArgs e)
    {
        string error = "";
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) error += "Введите имя.\n";
        if (!int.TryParse(AgeEntry.Text, out int age) || age < 6 || age > 100)
            error += "Введите возраст от 6 до 100.";
        ErrorLabel.Text = error.Trim();
        ErrorLabel.IsVisible = error.Length > 0;
        ResultLabel.IsVisible = error.Length == 0;
        if (error.Length > 0) return;

        string place = HomeRadio.IsChecked ? "Дома" : CinemaRadio.IsChecked ? "В кинотеатре" : "По-разному";
        string genre = GenrePicker.SelectedItem?.ToString() ?? "Не выбран";
        string comment = string.IsNullOrWhiteSpace(CommentEditor.Text) ? "Нет" : CommentEditor.Text.Trim();
        ResultLabel.Text = $"Имя: {NameEntry.Text.Trim()}\nВозраст: {age}\nЖанр: {genre}\n"
            + $"Место: {place}\nСубтитры: {(SubtitlesCheck.IsChecked ? "Да" : "Нет")}\n"
            + $"Компания: {(CompanyCheck.IsChecked ? "Да" : "Нет")}\n"
            + $"Новинки: {(NewsSwitch.IsToggled ? "Да" : "Нет")}\nДата: {WatchDate.Date:dd.MM.yyyy}\n"
            + $"Фильмов в месяц: {FilmsStepper.Value:F0}\nКомментарий: {comment}";
    }

    void OnClearClicked(object sender, EventArgs e)
    {
        NameEntry.Text = "";
        AgeEntry.Text = "";
        GenrePicker.SelectedIndex = -1;
        HomeRadio.IsChecked = true;
        CinemaRadio.IsChecked = false;
        BothRadio.IsChecked = false;
        SubtitlesCheck.IsChecked = false;
        CompanyCheck.IsChecked = false;
        NewsSwitch.IsToggled = false;
        WatchDate.Date = new DateTime(2026, 10, 30);
        FilmsStepper.Value = 2;
        CommentEditor.Text = "";
        ResultLabel.Text = "";
        ResultLabel.IsVisible = false;
        ErrorLabel.Text = "";
        ErrorLabel.IsVisible = false;
        ReadyLabel.IsVisible = false;
    }
}

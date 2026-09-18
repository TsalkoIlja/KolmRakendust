using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace KolmRakendust
{
    public class MatemaatikaForm : Form
    {
        // Поля для хранения надписей с математическими примерами
        private Label addLabel, subLabel, multLabel, divLabel;
        // Поля для хранения информационных меток (сообщение о результате, таймер, счетчик очков)
        private Label tulemusLabel, aegLabel, punktidLabel;
        // Поля ввода чисел для ответов пользователя на каждый из 4 примеров
        private NumericUpDown addInput, subInput, multInput, divInput;
        // Кнопка для старта игры
        private Button alustaNupp;
        // Выпадающий список выбора сложности
        private ComboBox raskusasteCombo;
        // Таймер для обратного отсчета времени
        private Timer taimer;

        // Переменные для операндов (чисел) каждого из примеров
        private int addA, addB, subA, subB, multA, multB, divA, divB;
        private int jaanudAega;
        private int punktid = 0;
        private Random rand = new Random();
        private TableLayoutPanel layoutPaneel;


        public MatemaatikaForm()
        {
            Text = "Matemaatiline test";
            Size = new Size(550, 680); // Увеличена высота формы для корректного отображения всех элементов
            StartPosition = FormStartPosition.CenterScreen; // Позиция окна по центру
            BackColor = Color.FromArgb(245, 247, 250);

            // Инициализация таблицы компоновки (2 столбца, 8 строк)
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(15)
            };

            // Разделение ширины таблицы: каждый столбец занимает по 50%
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Настройка высоты строк таблицы, чтобы ни один элемент не обрезался
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 0: Сложность и Таймер
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 1: +
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 2: -
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 3: *
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 4: /
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 5: Кнопка старта и Очки
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Строка 6: Результат
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F)); // Строка 7: Текстовое поле внизу

            // Метка для отображения времени
            aegLabel = new Label
            {
                Text = "Aeg: 40",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            // Метка для отображения набранных очков
            punktidLabel = new Label
            {
                Text = "Punktid: 0",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185)
            };

            // Выпадающий список уровней сложности
            raskusasteCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.None,
                Font = new Font("Segoe UI", 10)
            };
            raskusasteCombo.Items.AddRange(new object[] { "Kerge", "Keskmine", "Raske" });
            raskusasteCombo.SelectedIndex = 0;

            // Создание текстовых меток для математических операторов через вспомогательный метод
            addLabel = LooLabel("? + ? =");
            subLabel = LooLabel("? - ? =");
            multLabel = LooLabel("? × ? =");
            divLabel = LooLabel("? ÷ ? =");

            // Создание полей ввода ответов через вспомогательный метод
            addInput = LooInput();
            subInput = LooInput();
            multInput = LooInput();
            divInput = LooInput();

            // Создание и стилизация кнопки старта
            alustaNupp = new Button
            {
                Text = "Alusta mängu",
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.None
            };
            alustaNupp.FlatAppearance.BorderSize = 0;

            // Метка для вывода сообщений о победе или поражении
            tulemusLabel = new Label
            {
                Text = "",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            taimer = new Timer { Interval = 1000 }; // Интервал таймера 1 секунда (1000 мс)

            // Генерация примеров в зависимости от выбранного уровня сложности
            alustaNupp.Click += (s, e) =>
            {
                int maxNum = 20;
                jaanudAega = 40;

                // Настройка параметров в зависимости от выбранной сложности
                if (raskusasteCombo.SelectedIndex == 1) { maxNum = 50; jaanudAega = 30; }
                else if (raskusasteCombo.SelectedIndex == 2) { maxNum = 100; jaanudAega = 20; }

                addA = rand.Next(1, maxNum);
                addB = rand.Next(1, maxNum);
                addLabel.Text = $"{addA} + {addB} =";
                addInput.Value = 0;

                subA = rand.Next(1, maxNum);
                subB = rand.Next(1, subA); // Гарантируем положительный результат при вычитании
                subLabel.Text = $"{subA} - {subB} =";
                subInput.Value = 0;

                multA = rand.Next(2, maxNum / 2);
                multB = rand.Next(2, 10);
                multLabel.Text = $"{multA} × {multB} =";
                multInput.Value = 0;

                // Для деления сначала генерируем делитель и частное, чтобы избежать дробей
                divB = rand.Next(2, 10);
                int jagaja = rand.Next(1, maxNum / 2);
                divA = divB * jagaja;
                divLabel.Text = $"{divA} ÷ {divB} =";
                divInput.Value = 0;

                aegLabel.Text = $"Aeg: {jaanudAega}";
                tulemusLabel.Text = "";
                taimer.Start();
            };

            // Проверка ответов пользователя при изменении значений во вводимых полях
            EventHandler kontrolliVastuseid = (s, e) =>
            {
                if (taimer.Enabled &&
                    addInput.Value == addA + addB &&
                    subInput.Value == subA - subB &&
                    multInput.Value == multA * multB &&
                    divInput.Value == divA / divB)
                {
                    taimer.Stop();
                    punktid += 10 + jaanudAega; // Начисление бонуса за оставшееся время
                    punktidLabel.Text = $"Punktid: {punktid}";
                    tulemusLabel.Text = "Õige! Kõik ülesanded lahendatud!";
                    tulemusLabel.ForeColor = Color.FromArgb(39, 174, 96);
                }
            };

            addInput.ValueChanged += kontrolliVastuseid;
            subInput.ValueChanged += kontrolliVastuseid;
            multInput.ValueChanged += kontrolliVastuseid;
            divInput.ValueChanged += kontrolliVastuseid;

            // Отсчет времени
            taimer.Tick += (s, e) =>
            {
                jaanudAega--;
                aegLabel.Text = $"Aeg: {jaanudAega}";
                if (jaanudAega <= 0)
                {
                    taimer.Stop();
                    tulemusLabel.Text = "Aeg sai otsa!";
                    tulemusLabel.ForeColor = Color.FromArgb(192, 57, 43);
                }
            };

            // Размещение выборочных элементов в табличной сетке
            layoutPaneel.Controls.Add(raskusasteCombo, 0, 0);
            layoutPaneel.Controls.Add(aegLabel, 1, 0);

            // Добавление строк с примерами в сетку
            LisaRida(addLabel, addInput, 1);
            LisaRida(subLabel, subInput, 2);
            LisaRida(multLabel, multInput, 3);
            LisaRida(divLabel, divInput, 4);

            // Размещение кнопки старта и очков
            layoutPaneel.Controls.Add(alustaNupp, 0, 5);
            layoutPaneel.Controls.Add(punktidLabel, 1, 5);

            // Размещение статуса результата
            layoutPaneel.Controls.Add(tulemusLabel, 0, 6);
            layoutPaneel.SetColumnSpan(tulemusLabel, 2);

            // Информационное текстовое поле внизу формы
            TextBox infoBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical, // Включена прокрутка для гарантированного показа всего текста
                Text = "Vormi edasiarendused:\r\n" +
                       "1. Erinevad tehete tüübid (+, -, *, /).\r\n" +
                       "2. Taimer ja punktiarvestus kiiruse põhjal.\r\n" +
                       "3. Raskusastme valik (Kerge, Keskmine, Raske)."
            };
            layoutPaneel.Controls.Add(infoBox, 0, 7);
            layoutPaneel.SetColumnSpan(infoBox, 2);

            Controls.Add(layoutPaneel);
        }

        // Вспомогательный метод для создания однотипных текстовых меток с примерами
        private Label LooLabel(string tekst)
        {
            return new Label
            {
                Text = tekst,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Anchor = AnchorStyles.Right
            };
        }

        // Вспомогательный метод для создания элементов ввода чисел
        private NumericUpDown LooInput()
        {
            return new NumericUpDown
            {
                Width = 90,
                Font = new Font("Segoe UI", 11),
                Maximum = 10000,
                Anchor = AnchorStyles.Left
            };
        }

        // Вспомогательный метод добавления пары «Метка + Поле ввода» в указанную строку сетки
        private void LisaRida(Label lbl, NumericUpDown inp, int row)
        {
            layoutPaneel.Controls.Add(lbl, 0, row);
            layoutPaneel.Controls.Add(inp, 1, row);
        }
    }
}
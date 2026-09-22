using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace KolmRakendust
{
    public class MalumangForm : Form
    {
        // Объявление закрытых полей формы
        private TableLayoutPanel layoutPaneel; // Табличная панель для размещения карточек
        private List<string> ikoonid = new List<string> // Список символов шрифта Webdings, используемых в качестве икон (пары)
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };
        private Label esimeneVajutus = null; 
        private Label teineVajutus = null; 
        private Timer taimer; 
        private Timer mangTaimer;
        private Label staatusLabel; 
        private Label parimTulemusLabel; 
        private ComboBox taseCombo; 
        private Button startNupp; 

        private int kulunudAeg = 0; // Переменная для хранения прошедшего времени (в секундах)
        private int kaikudeArv = 0; // Переменная для подсчета количества сделанных ходов

        // Переменные для хранения лучших результатов (сохраняются между играми)
        private int parimAeg = int.MaxValue;
        private int parimadKaikud = int.MaxValue;

        private Random rand = new Random(); // Генератор случайных чисел для перемешивания карточек

        public MalumangForm() // Конструктор формы
        {
            Text = "Mälumäng"; // Установка заголовка окна
            Size = new Size(600, 680); // Размеры окна
            StartPosition = FormStartPosition.CenterScreen; // Отображение формы по центру экрана
            BackColor = Color.FromArgb(245, 247, 250); // Светлый фон окна

            TableLayoutPanel peaPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Заполнение всей формы
                RowCount = 3, // Три строки (верхняя панель, поле игры, нижняя инфо-панель)
                ColumnCount = 1 // Один столбец
            };
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Увеличена высота верхней панели для размещения рекорда
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 80F)); // Игровое поле занимает 80% оставшейся высоты
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F)); // Инфо-панель занимает 20% высоты

            // Верхняя панель с элементами управления
            FlowLayoutPanel yleminePaneel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            staatusLabel = new Label { Text = "Aeg: 0 s | Käigud: 0", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            // Создание текстовой метки для лучшего результата
            parimTulemusLabel = new Label { Text = "Parim: -", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.DarkGreen };

            // Выпадающий список уровней сложности
            taseCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            taseCombo.Items.AddRange(new object[] { "2x2 (Kerge)", "4x4 (Tavaline)" }); // Варианты сложности
            taseCombo.SelectedIndex = 1; // По умолчанию выбран уровень 4x4

            // Кнопка запуска новой игры
            startNupp = new Button { Text = "Uus mäng", AutoSize = true };
            startNupp.Click += (s, e) => AlustaMangu(); // При клике запускается метод AlustaMangu

            // Добавление элементов на верхнюю панель
            yleminePaneel.Controls.Add(taseCombo);
            yleminePaneel.Controls.Add(startNupp);
            yleminePaneel.Controls.Add(staatusLabel);
            yleminePaneel.Controls.Add(parimTulemusLabel); // Добавление метки рекорда

            peaPaneel.Controls.Add(yleminePaneel, 0, 0); // Размещение верхней панели в первой строке главного контейнера

            // Панель для игрового поля
            layoutPaneel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            peaPaneel.Controls.Add(layoutPaneel, 0, 1); // Размещение игрового поля во второй строке

            // Настройка таймера для задержки перед прятанием пары (750 мс)
            taimer = new Timer { Interval = 750 };
            taimer.Tick += (s, e) =>
            {
                taimer.Stop(); // Остановка таймера
                esimeneVajutus.ForeColor = esimeneVajutus.BackColor; // Прячем иконку первой карточки
                teineVajutus.ForeColor = teineVajutus.BackColor; // Прячем иконку второй карточки
                esimeneVajutus = null; // Сброс первой карточки
                teineVajutus = null; // Сброс второй карточки
            };

            // Настройка секундного таймера для общего времени игры
            mangTaimer = new Timer { Interval = 1000 };
            mangTaimer.Tick += (s, e) =>
            {
                kulunudAeg++; // Увеличение счетчика секунд
                UuendaStaatust(); // Обновление текста со статистикой
            };

            // Информационное текстовое поле внизу формы
            TextBox infoBox = new TextBox
            {
                Multiline = true, // Многострочный режим
                ReadOnly = true, // Только для чтения
                Dock = DockStyle.Fill, // Заполнение отведенной области
                Text = "Vormi edasiarendused:\r\n" +
                       "1. Tegelikud pildid sümbolite asemel (või Webdings ikoonid).\r\n" +
                       "2. Taimer ja käikude/punktide loendur.\r\n" +
                       "3. Erinevad tasemed (2x2 ja 4x4 ruudustikud).\r\n" +
                       "4. Parima tulemuse salvestamine seansi jooksul."
            };
            peaPaneel.Controls.Add(infoBox, 0, 2); // Размещение в третьей строке

            Controls.Add(peaPaneel); // Добавление главного контейнера на форму
            AlustaMangu(); // Автоматический запуск новой игры при открытии
        }

        // Метод начала / перезапуска игры
        private void AlustaMangu()
        {
            mangTaimer.Stop();
            kulunudAeg = 0; // Сброс времени текущей игры
            kaikudeArv = 0; // Сброс счетчика ходов текущей игры
            UuendaStaatust(); // Обновление статуса на экране
            esimeneVajutus = null; // Сброс выделенной первой карточки
            teineVajutus = null; // Сброс выделенной второй карточки

            // Очистка предыдущей сетки элементов
            layoutPaneel.Controls.Clear();
            layoutPaneel.ColumnStyles.Clear();
            layoutPaneel.RowStyles.Clear();

            // Определение размера сетки (2x2 или 4x4)
            int suurus = taseCombo.SelectedIndex == 0 ? 2 : 4;
            layoutPaneel.ColumnCount = suurus;
            layoutPaneel.RowCount = suurus;

            // Настройка пропорциональных размеров столбцов и строк
            for (int i = 0; i < suurus; i++)
            {
                layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / suurus));
                layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / suurus));
            }

            int kokku = suurus * suurus; // Общее количество карточек
            List<string> kasutatavad = ikoonid.GetRange(0, kokku);
            List<string> koopia = new List<string>(kasutatavad);

            // Генерация и распределение карточек по сетке
            for (int i = 0; i < kokku; i++)
            {
                int idx = rand.Next(koopia.Count); // Выбор случайного индекса
                string ikoon = koopia[idx]; // Получение иконки
                koopia.RemoveAt(idx); // Удаление выбранной иконки из доступных

                // Создание элемента карточки (Label)
                Label kaart = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = ikoon, // Символ из шрифта Webdings
                    Font = new Font("Webdings", suurus == 2 ? 60 : 36, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(155, 89, 182),
                    ForeColor = Color.FromArgb(155, 89, 182), // Текст скрыт
                    Margin = new Padding(3),
                    BorderStyle = BorderStyle.FixedSingle
                };

                kaart.Click += Kaart_Click; // Подключение обработчика клика
                layoutPaneel.Controls.Add(kaart); // Добавление карточки в сетку
            }

            mangTaimer.Start(); // Запуск отсчета времени
        }

        // Обработчик события клика по карточке
        private void Kaart_Click(object sender, EventArgs e)
        {
            if (taimer.Enabled) return;

            Label vajutatud = sender as Label;
            if (vajutatud == null || vajutatud.ForeColor == Color.White) return;

            if (esimeneVajutus == null)
            {
                esimeneVajutus = vajutatud;
                esimeneVajutus.ForeColor = Color.White;
                return;
            }

            teineVajutus = vajutatud;
            teineVajutus.ForeColor = Color.White;
            kaikudeArv++; // Увеличиваем счетчик ходов
            UuendaStaatust(); // Обновляем инфо о ходах и времени

            if (esimeneVajutus.Text == teineVajutus.Text)
            {
                esimeneVajutus = null;
                teineVajutus = null;
                KontrolliVoitu(); // Проверяем, не завершена ли игра
            }
            else
            {
                taimer.Start();
            }
        }

        // Обновление строки состояния
        private void UuendaStaatust()
        {
            staatusLabel.Text = $"Aeg: {kulunudAeg} s | Käigud: {kaikudeArv}";
        }

        // Проверка условия победы и обновление рекорда
        private void KontrolliVoitu()
        {
            foreach (Control control in layoutPaneel.Controls)
            {
                Label kaart = control as Label;
                if (kaart != null && kaart.ForeColor == kaart.BackColor)
                    return;
            }

            mangTaimer.Stop(); // Остановка времени после победы

            // Логика проверки и сохранения лучшего результата
            if (kulunudAeg < parimAeg)
            {
                parimAeg = kulunudAeg;
                parimadKaikud = kaikudeArv;
                parimTulemusLabel.Text = $"Parim: {parimAeg} s ({parimadKaikud} käiku)"; 
            }

            // Вывод диалогового окна с победными результатами и текущим рекордом
            MessageBox.Show($"Võit!\r\nPraegune aeg: {kulunudAeg} s, käike: {kaikudeArv}\r\nParim tulemus: {parimAeg} s", "Palju õnne!");
        }
    }
}
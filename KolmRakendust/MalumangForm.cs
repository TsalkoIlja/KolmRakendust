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
        private Label esimeneVajutus = null; // Ссылка на первую открытую карточку
        private Label teineVajutus = null; // Ссылка на вторую открытую карточку
        private Timer taimer; // Таймер задержки перед закрытием несовпавших карточек
        private Timer mangTaimer; // Таймер для отсчета общего времени игры
        private Label staatusLabel; // Текстовая метка для вывода времени и количества ходов
        private ComboBox taseCombo; // Выпадающий список выбора уровня сложности
        private Button startNupp; // Кнопка перезапуска игры

        private int kulunudAeg = 0; // Переменная для хранения прошедшего времени (в секундах)
        private int kaikudeArv = 0; // Переменная для подсчета количества сделанных ходов
        private Random rand = new Random(); // Генератор случайных чисел для перемешивания карточек

        public MalumangForm() // Конструктор формы
        {
            Text = "Mälumäng"; // Установка заголовка окна
            Size = new Size(600, 650); // Размеры окна (600x650)
            StartPosition = FormStartPosition.CenterScreen; // Отображение формы по центру экрана
            BackColor = Color.FromArgb(245, 247, 250); // Светлый фон окна

            
            TableLayoutPanel peaPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Заполнение всей формы
                RowCount = 3, // Три строки (верхняя панель, поле игры, нижняя инфо-панель)
                ColumnCount = 1 // Один столбец
            };
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Высота верхней панели — 40 пикселей
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 80F)); // Игровое поле занимает 80% оставшейся высоты
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F)); // Инфо-панель занимает 20% высоты

            // Верхняя панель с элементами управления
            FlowLayoutPanel yleminePaneel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            staatusLabel = new Label { Text = "Aeg: 0 s | Käigud: 0", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

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

            peaPaneel.Controls.Add(yleminePaneel, 0, 0); // Размещение верхней панели в первой строке главного контейнера

            // Панель для игрового поля
            layoutPaneel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            peaPaneel.Controls.Add(layoutPaneel, 0, 1); // Размещение игрового поля во второй строке

            // Настройка таймера для задержки перед прятанием пары (750 мс)
            taimer = new Timer { Interval = 750 };
            taimer.Tick += (s, e) =>
            {
                taimer.Stop(); // Остановка таймера
                esimeneVajutus.ForeColor = esimeneVajutus.BackColor; // Прячем иконку первой карточки (цвет текста = цвет фона)
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
                       "3. Erinevad tasemed (2x2 ja 4x4 ruudustikud)."
            };
            peaPaneel.Controls.Add(infoBox, 0, 2); // Размещение в третьей строке

            Controls.Add(peaPaneel); // Добавление главного контейнера на форму
            AlustaMangu(); // Автоматический запуск новой игры при открытии
        }

        // Метод начала / перезапуска игры
        private void AlustaMangu()
        {
            mangTaimer.Stop(); 
            kulunudAeg = 0; // Сброс времени
            kaikudeArv = 0; // Сброс счетчика ходов
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
                int idx = rand.Next(koopia.Count); // Выбор случайного индекса из оставшихся
                string ikoon = koopia[idx]; // Получение иконки
                koopia.RemoveAt(idx); // Удаление выбранной иконки из доступных

                // Создание элемента карточки (Label)
                Label kaart = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = ikoon, // Символ из шрифта Webdings
                    Font = new Font("Webdings", suurus == 2 ? 60 : 36, FontStyle.Bold), // Размер шрифта зависит от размера сетки
                    TextAlign = ContentAlignment.MiddleCenter, // Выравнивание текста по центру
                    BackColor = Color.FromArgb(155, 89, 182), // Фиолетовый фон карточки
                    ForeColor = Color.FromArgb(155, 89, 182), // Текст изначально скрыт (соответствует цвету фона)
                    Margin = new Padding(3), // Внешние отступы
                    BorderStyle = BorderStyle.FixedSingle // Одинарная рамка карточки
                };

                kaart.Click += Kaart_Click; // Подключение обработчика клика
                layoutPaneel.Controls.Add(kaart); // Добавление карточки в сетку
            }

            mangTaimer.Start(); // Запуск отсчета времени
        }

        // Обработчик события клика по карточке
        private void Kaart_Click(object sender, EventArgs e)
        {
            // Игнорировать клики, если активен таймер задержки (прятание карточек)
            if (taimer.Enabled) return;

            Label vajutatud = sender as Label; // Получение ссылки на нажатую карточку
            // Игнорировать клик, если это не Label или если карточка уже открыта (цвет текста белый)
            if (vajutatud == null || vajutatud.ForeColor == Color.White) return;

            // Если это первая открытая карточка в парах
            if (esimeneVajutus == null)
            {
                esimeneVajutus = vajutatud;
                esimeneVajutus.ForeColor = Color.White; 
                return;
            }

            // Если это вторая открытая карточка
            teineVajutus = vajutatud;
            teineVajutus.ForeColor = Color.White; 
            kaikudeArv++; // Увеличиваем счетчик ходов
            UuendaStaatust(); // Обновляем инфо о ходах и времени

            // Проверка совпадения символов
            if (esimeneVajutus.Text == teineVajutus.Text)
            {
                esimeneVajutus = null; // Очищаем первую карточку (пара найдена, остается открытой)
                teineVajutus = null; // Очищаем вторую карточку
                KontrolliVoitu(); // Проверяем, не завершена ли игра
            }
            else
            {
                taimer.Start(); // Карточки не совпали — запускаем таймер для их скрытия
            }
        }

        // Обновление строки состояния
        private void UuendaStaatust()
        {
            staatusLabel.Text = $"Aeg: {kulunudAeg} s | Käigud: {kaikudeArv}";
        }

        // Проверка условия победы
        private void KontrolliVoitu()
        {
            // Перебираем все карточки на игровой панели
            foreach (Control control in layoutPaneel.Controls)
            {
                Label kaart = control as Label;
                // Если хоть у одной карточки цвет текста совпадает с цветом фона (она закрыта), выходим
                if (kaart != null && kaart.ForeColor == kaart.BackColor)
                    return;
            }

            mangTaimer.Stop(); // Остановка времени после победы
            // Вывод диалогового окна с победными результатами
            MessageBox.Show($"Võit! Aeg: {kulunudAeg} sekundit, käike: {kaikudeArv}", "Palju õnne!");
        }
    }
}
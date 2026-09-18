using System; 
using System.Collections.Generic; 
using System.Drawing;
using System.Drawing.Imaging; 
using System.IO; 
using System.Linq; 
using System.Windows.Forms; 
using Timer = System.Windows.Forms.Timer; 

namespace KolmRakendust 
{
    public class PildivaatjaForm : Form 
    {
        // Поля формы для графических элементов управления
        private PictureBox pictureControl; // Элемент для отображения картинки
        private CheckBox stretchCheck; // Флажок масштабирования/растягивания картинки
        private Button showBtn, clearBtn, colorBtn, closeBtn, saveBtn, slideBtn; // Основные кнопки
        private Button eelmineBtn, jargmineBtn; // Кнопки навигации «Назад» и «Вперед»
        private TableLayoutPanel layoutPaneel; // Главная табличная панель компоновки
        private FlowLayoutPanel buttonPaneel; // Панель для удобного размещения кнопок в ряд
        private Timer slideTimer; 

        private List<string> pildidTee = new List<string>(); // Список путей ко всем картинкам в открытой папке
        private int praeguneIndeks = -1; // Индекс текущей отображаемой картинки в списке

        public PildivaatjaForm() // Конструктор формы
        {
            Text = "Pildi vaatamise programm"; // Установка заголовка окна
            Size = new Size(800, 600); // Начальные размеры окна (800x600)
            StartPosition = FormStartPosition.CenterScreen; // Отображение формы по центру экрана

            // Создание таблицы сетки элементов (2 столбца, 3 строки)
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Заполнение всего пространства формы
                ColumnCount = 2, // 2 столбца
                RowCount = 3 // 3 строки
            };
            // Пропорции ширины столбцов (20% и 80%)
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            // Пропорции высоты строк (70% - область картинки, 12% - кнопки, 18% - инфо-поле)
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));

            // Элемент отображения изображения
            pictureControl = new PictureBox
            {
                Dock = DockStyle.Fill, // Растягивание на всю ячейку
                BorderStyle = BorderStyle.Fixed3D, // Объемная рамка
                SizeMode = PictureBoxSizeMode.CenterImage // По умолчанию картинка по центру без искажения
            };
            layoutPaneel.Controls.Add(pictureControl, 0, 0); // Помещаем в ячейку (0,0)
            layoutPaneel.SetColumnSpan(pictureControl, 2); // Картинка занимает оба столбца

            // Флажок (CheckBox) для включения растягивания картинки
            stretchCheck = new CheckBox
            {
                Text = "Venita pilt", // Текст «Растянуть картинку»
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            // Переключение режима отображения картинки при изменении состояния галочки
            stretchCheck.CheckedChanged += (s, e) =>
            {
                pictureControl.SizeMode = stretchCheck.Checked
                    ? PictureBoxSizeMode.StretchImage // Если галочка стоит — растягивать
                    : PictureBoxSizeMode.CenterImage; // Если нет — по центру
            };
            layoutPaneel.Controls.Add(stretchCheck, 0, 1); // Добавляем в 1-ю строку, 0-й столбец

            // Контейнер для горизонтального размещения кнопок управления
            buttonPaneel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft, // Размещение кнопок справа налево
                WrapContents = false // Запрет переноса кнопок на новую строку
            };

            // Кнопка закрытия окна
            closeBtn = LooNupp("Sulge", Color.FromArgb(149, 165, 166));
            closeBtn.Click += (s, e) => Close(); // Закрытие текущей формы

            // Кнопка очистки изображения
            clearBtn = LooNupp("Eemalda", Color.FromArgb(231, 76, 60));
            clearBtn.Click += (s, e) =>
            {
                pictureControl.Image = null; // Удаляем изображение
                pildidTee.Clear(); // Очищаем список путей файлов
                praeguneIndeks = -1; // Сбрасываем индекс
                slideTimer.Stop(); // Останавливаем слайд-шоу, если оно шло
            };

            // Кнопка изменения цвета фона под картинкой
            colorBtn = LooNupp("Taustavärv", Color.FromArgb(155, 89, 182));
            colorBtn.Click += (s, e) =>
            {
                using (ColorDialog colorDlg = new ColorDialog()) // Диалог выбора цвета
                {
                    if (colorDlg.ShowDialog() == DialogResult.OK)
                        pictureControl.BackColor = colorDlg.Color; // Применение выбранного цвета к фону PictureBox
                }
            };

            // Кнопка сохранения текущего изображения
            saveBtn = LooNupp("Salvesta teise formaati", Color.FromArgb(230, 126, 34));
            saveBtn.Click += (s, e) => SalvestaPilt(); // Вызов метода сохранения

            // Кнопка слайд-шоу и настройка таймера
            slideBtn = LooNupp("Slaidishow", Color.FromArgb(241, 196, 15));
            slideTimer = new Timer { Interval = 2000 }; // Интервал смены картинок — 2 секунды (2000 мс)

            // Переключение на следующее изображение при каждом срабатывании таймера
            slideTimer.Tick += (s, e) => KuvaPilt(1);

            // Переключение работы слайд-шоу (старт/стоп) по нажатию кнопки
            slideBtn.Click += (s, e) =>
            {
                if (slideTimer.Enabled) // Если уже запущено
                {
                    slideTimer.Stop(); 
                    slideBtn.BackColor = Color.FromArgb(241, 196, 15); // Возвращаем желтый цвет кнопки
                }
                else if (pildidTee.Count > 0) // Если есть загруженные картинки
                {
                    slideTimer.Start(); 
                    slideBtn.BackColor = Color.FromArgb(46, 204, 113); // Меняем цвет кнопки на зеленый
                }
            };

            // Кнопки навигации по галерее
            eelmineBtn = LooNupp("<", Color.FromArgb(52, 73, 94)); 
            jargmineBtn = LooNupp(">", Color.FromArgb(52, 73, 94)); 
            eelmineBtn.Click += (s, e) => KuvaPilt(-1); // На шаг назад
            jargmineBtn.Click += (s, e) => KuvaPilt(1); // На шаг вперед

            // Кнопка открытия изображения через диалог выбора файлов
            showBtn = LooNupp("Ava pilt", Color.FromArgb(52, 152, 219));
            showBtn.Click += (s, e) =>
            {
                using (OpenFileDialog openDlg = new OpenFileDialog()) // Диалог открытия файла
                {
                    openDlg.Filter = "Pildid|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Фильтр расширений файлов
                    if (openDlg.ShowDialog() == DialogResult.OK)
                    {
                        string valitudFail = openDlg.FileName; // Путь к выбранному файлу
                        string kaust = Path.GetDirectoryName(valitudFail); // Каталог, где лежит файл

                        // Находим все поддерживаемые графические файлы в этой же папке
                        string[] laendid = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        pildidTee = Directory.GetFiles(kaust)
                            .Where(f => laendid.Contains(Path.GetExtension(f).ToLower())) // Фильтрация по расширению
                            .ToList();

                        praeguneIndeks = pildidTee.IndexOf(valitudFail); // Определение индекса открытого файла
                        pictureControl.Load(valitudFail); // Загрузка и показ изображения
                    }
                }
            };

            // Добавление всех кнопок в панель кнопок
            buttonPaneel.Controls.Add(closeBtn);
            buttonPaneel.Controls.Add(clearBtn);
            buttonPaneel.Controls.Add(saveBtn);
            buttonPaneel.Controls.Add(slideBtn);
            buttonPaneel.Controls.Add(colorBtn);
            buttonPaneel.Controls.Add(showBtn);
            buttonPaneel.Controls.Add(jargmineBtn);
            buttonPaneel.Controls.Add(eelmineBtn);

            layoutPaneel.Controls.Add(buttonPaneel, 1, 1); // Добавление панели кнопок в сетку (строка 1, столбец 1)

            // Информационное текстовое поле внизу окна
            TextBox infoBox = new TextBox
            {
                Multiline = true, // Многострочный режим
                ReadOnly = true, // Запрет редактирования
                Dock = DockStyle.Fill,
                Text = "Vormi edasiarendused:\r\n" +
                       "1. Taustavärvi muutmine (ColorDialog).\r\n" +
                       "2. Automaatne slaidishow taimeriga.\r\n" +
                       "3. Pildi salvestamine teise formaati (PNG, JPEG, BMP)."
            };
            layoutPaneel.Controls.Add(infoBox, 0, 2); // Размещение в 2-й строке
            layoutPaneel.SetColumnSpan(infoBox, 2); // Растягивание на 2 столбца

            Controls.Add(layoutPaneel); // Добавление главного макета на форму
        }

        // Метод для переключения текущего изображения (вперед/назад) по кругу
        private void KuvaPilt(int samm)
        {
            if (pildidTee == null || pildidTee.Count == 0) return; // Если список пуст — ничего не делаем

            praeguneIndeks += samm; // Изменяем текущий индекс
            if (praeguneIndeks >= pildidTee.Count) praeguneIndeks = 0; // Переход в начало списка при выходе за границы
            else if (praeguneIndeks < 0) praeguneIndeks = pildidTee.Count - 1; // Переход в конец списка при выходе назад

            pictureControl.Load(pildidTee[praeguneIndeks]); // Загрузка нового файла
        }

        // Метод для сохранения открытого изображения в выбранном формате
        private void SalvestaPilt()
        {
            if (pictureControl.Image == null) return; // Проверка, что картинка загружена

            using (SaveFileDialog saveDlg = new SaveFileDialog()) // Диалог сохранения файла
            {
                saveDlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp"; // Форматы сохранения
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png; // Формат по умолчанию — PNG
                    switch (saveDlg.FilterIndex) // Определение формата по выбору пользователя
                    {
                        case 2: format = ImageFormat.Jpeg; break; // JPEG
                        case 3: format = ImageFormat.Bmp; break; // BMP
                    }
                    pictureControl.Image.Save(saveDlg.FileName, format); // Сохранение файла на диск
                }
            }
        }

        // Вспомогательный метод для унифицированного создания красивых кнопок
        private Button LooNupp(string tekst, Color varv)
        {
            Button btn = new Button
            {
                Text = tekst, 
                AutoSize = true, // Автоматический размер по ширине текста
                Height = 30, // Фиксированная высота
                BackColor = varv, 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat, // Плоский стиль отображения
                Font = new Font("Segoe UI", 9, FontStyle.Bold), // Шрифт
                Margin = new Padding(2) // Отступы вокруг кнопки
            };
            btn.FlatAppearance.BorderSize = 0; // Убираем внешнюю рамку кнопки
            return btn; // Возврат готовой кнопки
        }
    }
}
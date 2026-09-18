using System; 
using System.Drawing; 
using System.Windows.Forms; 

namespace KolmRakendust 
{
    public partial class Form1 : Form 
    {
        // Объявление закрытых полей (переменных) для элементов управления
        private Button piltNupp; 
        private Button matemaatikaNupp; 
        private Button malumangNupp; 
        private TableLayoutPanel layoutPaneel; 

        public Form1() 
        {
            Text = "Peamenüü - Kolm Rakendust"; // Установка заголовка окна
            Size = new Size(550, 500); // Задание размеров окна (ширина: 550, высота: 500 пикселей)
            StartPosition = FormStartPosition.CenterScreen; // Отображение окна по центру экрана при запуске
            BackColor = Color.FromArgb(245, 247, 250); // Установка светлого фона формы с помощью RGB-цвета

            // Создаем сеточную панель (TableLayoutPanel) для автоматического выравнивания кнопок
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Заполнение панелью всего пространства формы
                ColumnCount = 1, // Задание количества столбцов (1 столбец)
                RowCount = 3, // Задание количества строк (3 строки)
                Padding = new Padding(20) // Внутренний отступ от краев панели в 20 пикселей
            };

            // Настройка высоты строк: каждая из 3 строк занимает 33.3% от высоты панели
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F)); 
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F)); 
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F)); 

            // Создание и настройка трех кнопок с помощью вспомогательного метода LooKenaNupp
            piltNupp = LooKenaNupp("Pildi vaatamise programm", Color.FromArgb(52, 152, 219)); 
            matemaatikaNupp = LooKenaNupp("Matemaatiline mäng", Color.FromArgb(46, 204, 113)); 
            malumangNupp = LooKenaNupp("Mälumäng", Color.FromArgb(155, 89, 182)); 

            // Привязка событий нажатия на кнопки (при клике создается и открывается соответствующая форма)
            piltNupp.Click += (s, e) => new PildivaatjaForm().Show(); 
            matemaatikaNupp.Click += (s, e) => new MatemaatikaForm().Show();
            malumangNupp.Click += (s, e) => new MalumangForm().Show(); 

            // Добавление созданных кнопок в ячейки табличной панели
            layoutPaneel.Controls.Add(piltNupp, 0, 0); 
            layoutPaneel.Controls.Add(matemaatikaNupp, 0, 1); 
            layoutPaneel.Controls.Add(malumangNupp, 0, 2); 

            Controls.Add(layoutPaneel); // Добавление сформированной панели на главную форму
        }

        // Вспомогательный метод для создания и красивого оформления кнопок
        private Button LooKenaNupp(string tekst, Color taustaVarv)
        {
            var nupp = new Button // Создание нового экземпляра кнопки
            {
                Text = tekst, // Установка текста на кнопке
                Size = new Size(280, 55), // Задание стандартного размера кнопки (280x55)
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), // Задание шрифта: Segoe UI, размер 11, жирный
                ForeColor = Color.White, 
                BackColor = taustaVarv, // Цвет фона — переданный в параметре цвет
                FlatStyle = FlatStyle.Flat, // Плоский стиль отображения кнопки (без 3D- рамок)
                Anchor = AnchorStyles.None // Отмена привязки к краям, чтобы кнопка оставалась по центру ячейки
            };

            nupp.FlatAppearance.BorderSize = 0; // Убираем внешнюю рамку вокруг кнопки

            return nupp; // Возвращаем полностью настроенную кнопку
        }
    }
}

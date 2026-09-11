using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public partial class Form1 : Form
    {
        // Объявляем кнопки главного меню
        private Button piltNupp;
        private Button matemaatikaNupp;
        private Button malumangNupp;
        private TableLayoutPanel layoutPaneel;
        public Form1()
        {
            // Настройки самого окна
            Text = "Peamenüü";
            Size = new Size(500, 450);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250); // Приятный светлый фон

            // Создаем сетку для выравнивания кнопок строго по центру
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            // Задаем пропорции строк (равномерно по центру)
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            // Функция для красивой стилизации кнопок
            Button LooKenaNupp(string tekst, Color taustaVarv)
            {
                var nupp = new Button
                {
                    Text = tekst,
                    Size = new Size(260, 50),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = taustaVarv,
                    FlatStyle = FlatStyle.Flat,
                    Anchor = AnchorStyles.None // Держит кнопку по центру ячейки
                };
                nupp.FlatAppearance.BorderSize = 0;
                return nupp;
            }

            // Создаем кнопки с современным оформлением
            piltNupp = LooKenaNupp("Pildi vaatamise programm", Color.FromArgb(52, 152, 219));
            matemaatikaNupp = LooKenaNupp("Matemaatiline mäng", Color.FromArgb(46, 204, 113));
            malumangNupp = LooKenaNupp("Mälumäng", Color.FromArgb(155, 89, 182));

            // События перехода к формам
            piltNupp.Click += (s, e) => new PildivaatjaForm().Show();
            matemaatikaNupp.Click += (s, e) => new MatemaatikaForm().Show();
            malumangNupp.Click += (s, e) => new MalumangForm().Show();

            // Добавляем кнопки в сетку
            layoutPaneel.Controls.Add(piltNupp, 0, 0);
            layoutPaneel.Controls.Add(matemaatikaNupp, 0, 1);
            layoutPaneel.Controls.Add(malumangNupp, 0, 2);

            // Добавляем сетку на форму
            Controls.Add(layoutPaneel);
        }
    }
}

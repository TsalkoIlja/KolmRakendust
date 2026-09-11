using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class MalumangForm : Form
    {
        // Игровое поле, иконки, выбранные карточки и таймеры
        private TableLayoutPanel layoutPaneel;
        private List<string> ikoonid = new List<string>
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };
        private Label esimeneVajutus = null;
        private Label teineVajutus = null;
        private System.Windows.Forms.Timer taimer;
        private Random rand = new Random();

        public MalumangForm()
        {
            // Параметры окна
            Text = "Mälumäng";
            Size = new Size(550, 550);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);

            // Инициализация сетки 4х4
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                Padding = new Padding(15)
            };

            for (int i = 0; i < 4; i++)
            {
                layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            }

            // Настройка таймера для задержки закрытия несовпадающих карточек
            taimer = new System.Windows.Forms.Timer { Interval = 750 };
            taimer.Tick += (s, e) =>
            {
                taimer.Stop();
                esimeneVajutus.ForeColor = esimeneVajutus.BackColor;
                teineVajutus.ForeColor = teineVajutus.BackColor;
                esimeneVajutus = null;
                teineVajutus = null;
            };

            // Заполнение сетки карточками
            LooKaardid();

            Controls.Add(layoutPaneel);
        }

        private void LooKaardid()
        {
            // Перемешиваем список иконок
            List<string> koopiaIkoonid = new List<string>(ikoonid);

            for (int i = 0; i < 16; i++)
            {
                int suvalineIndeks = rand.Next(koopiaIkoonid.Count);
                string ikoon = koopiaIkoonid[suvalineIndeks];
                koopiaIkoonid.RemoveAt(suvalineIndeks);

                Label kaart = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = ikoon,
                    Font = new Font("Webdings", 48, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(155, 89, 182),
                    ForeColor = Color.FromArgb(155, 89, 182), // Скрываем текст (цвет текста равен цвету фона)
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Обработка клика по карточке
                kaart.Click += Kaart_Click;

                layoutPaneel.Controls.Add(kaart);
            }
        }

        private void Kaart_Click(object sender, EventArgs e)
        {
            // Если таймер работает, игнорируем новые клики
            if (taimer.Enabled) return;

            Label vajutatudKaart = sender as Label;

            if (vajutatudKaart == null || vajutatudKaart.ForeColor == Color.White)
                return; // Игнорируем уже открытые карточки

            // Открываем первую карточку
            if (esimeneVajutus == null)
            {
                esimeneVajutus = vajutatudKaart;
                esimeneVajutus.ForeColor = Color.White;
                return;
            }

            // Открываем вторую карточку
            teineVajutus = vajutatudKaart;
            teineVajutus.ForeColor = Color.White;

            // Проверка на совпадение
            if (esimeneVajutus.Text == teineVajutus.Text)
            {
                esimeneVajutus = null;
                teineVajutus = null;
                KontrolliVoitu();
            }
            else
            {
                taimer.Start(); // Запускаем задержку, чтобы скрывать обратно
            }
        }

        private void KontrolliVoitu()
        {
            // Проверяем, открыты ли все карточки
            foreach (Control control in layoutPaneel.Controls)
            {
                Label kaart = control as Label;
                if (kaart != null && kaart.ForeColor == kaart.BackColor)
                    return; // Нашли закрытую карточку
            }

            MessageBox.Show("Palju õnne! Leidsid kõik paarid!", "Võit!");
            Close();
        }
    }
}
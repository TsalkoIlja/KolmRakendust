using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class MatemaatikaForm : Form
    {
        // Поля ввода, метки текста, таймер и переменные для расчета
        private Label addLabel, subLabel, multLabel, divLabel;
        private Label tulemusLabel, aegLabel;
        private NumericUpDown addInput, subInput, multInput, divInput;
        private Button alustaNupp;
        private System.Windows.Forms.Timer taimer; // Явно указываем WinForms-таймер

        private int addA, addB, subA, subB, multA, multB, divA, divB;
        private int jaanudAega;
        private Random rand = new Random();

        // Панели для макета
        private TableLayoutPanel layoutPaneel;

        public MatemaatikaForm()
        {
            // Параметры окна
            Text = "Matemaatiline test";
            Size = new Size(500, 500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250); // Светлый современный фон

            // Главная сетка макета для центрирования
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(20)
            };

            // Настройка колонок: Левое число/знак | Поле ввода
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Инициализация элементов
            aegLabel = new Label
            {
                Text = "Aeg: 40", // Изменено на 40
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            addLabel = LooLabel("? + ? =");
            subLabel = LooLabel("? - ? =");
            multLabel = LooLabel("? × ? =");
            divLabel = LooLabel("? ÷ ? =");

            addInput = LooInput();
            subInput = LooInput();
            multInput = LooInput();
            divInput = LooInput();

            alustaNupp = new Button
            {
                Text = "Alusta mängu",
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.None
            };
            alustaNupp.FlatAppearance.BorderSize = 0;

            tulemusLabel = new Label
            {
                Text = "",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            taimer = new System.Windows.Forms.Timer { Interval = 1000 };

            // Старт игры
            alustaNupp.Click += (s, e) =>
            {
                // Сложение
                addA = rand.Next(1, 51);
                addB = rand.Next(1, 51);
                addLabel.Text = $"{addA} + {addB} =";
                addInput.Value = 0;

                // Вычитание
                subA = rand.Next(1, 51);
                subB = rand.Next(1, subA); // Чтобы не было отрицательных чисел
                subLabel.Text = $"{subA} - {subB} =";
                subInput.Value = 0;

                // Умножение
                multA = rand.Next(2, 11);
                multB = rand.Next(2, 11);
                multLabel.Text = $"{multA} × {multB} =";
                multInput.Value = 0;

                // Деление
                divB = rand.Next(2, 11);
                int jagaja = rand.Next(1, 11);
                divA = divB * jagaja; // Чтобы делилось без остатка
                divLabel.Text = $"{divA} ÷ {divB} =";
                divInput.Value = 0;

                jaanudAega = 40; // Установлено 40 секунд
                aegLabel.Text = $"Aeg: {jaanudAega}";
                tulemusLabel.Text = "";
                taimer.Start();
            };

            // Проверка правильности ответов
            EventHandler kontrolliVastuseid = (s, e) =>
            {
                if (taimer.Enabled &&
                    addInput.Value == addA + addB &&
                    subInput.Value == subA - subB &&
                    multInput.Value == multA * multB &&
                    divInput.Value == divA / divB)
                {
                    taimer.Stop();
                    tulemusLabel.Text = "Õige! Kõik ülesanded lahendatud!";
                    tulemusLabel.ForeColor = Color.FromArgb(39, 174, 96);
                }
            };

            addInput.ValueChanged += kontrolliVastuseid;
            subInput.ValueChanged += kontrolliVastuseid;
            multInput.ValueChanged += kontrolliVastuseid;
            divInput.ValueChanged += kontrolliVastuseid;

            // Отсчет обратного времени
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

            // Добавляем элементы в сетку макета
            layoutPaneel.Controls.Add(aegLabel, 0, 0);
            layoutPaneel.SetColumnSpan(aegLabel, 2);

            LisaRida(addLabel, addInput, 1);
            LisaRida(subLabel, subInput, 2);
            LisaRida(multLabel, multInput, 3);
            LisaRida(divLabel, divInput, 4);

            layoutPaneel.Controls.Add(alustaNupp, 0, 5);
            layoutPaneel.SetColumnSpan(alustaNupp, 2);

            layoutPaneel.Controls.Add(tulemusLabel, 0, 6);
            layoutPaneel.SetColumnSpan(tulemusLabel, 2);

            Controls.Add(layoutPaneel);
        }

        private Label LooLabel(string tekst)
        {
            return new Label
            {
                Text = tekst,
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Anchor = AnchorStyles.Right
            };
        }

        private NumericUpDown LooInput()
        {
            return new NumericUpDown
            {
                Width = 100,
                Font = new Font("Segoe UI", 14),
                Maximum = 1000,
                Anchor = AnchorStyles.Left
            };
        }

        private void LisaRida(Label lbl, NumericUpDown inp, int row)
        {
            layoutPaneel.Controls.Add(lbl, 0, row);
            layoutPaneel.Controls.Add(inp, 1, row);
        }
    }
}
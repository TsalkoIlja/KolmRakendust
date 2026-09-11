using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class PildivaatjaForm : Form
    {
        private PictureBox pictureControl;
        private CheckBox stretchCheck;
        private Button showBtn, clearBtn, colorBtn, closeBtn;
        private Button eelmineBtn, jargmineBtn; // Кнопки переключения картинок
        private TableLayoutPanel layoutPaneel;
        private FlowLayoutPanel buttonPaneel;

        // Список файлов в выбранной папке и индекс текущего файла
        private List<string> pildidTee = new List<string>();
        private int praeguneIndeks = -1;

        public PildivaatjaForm()
        {
            // Настройка окна
            Text = "Pildi vaatamise programm";
            Size = new Size(750, 480); // Немного увеличили ширину для удобства
            StartPosition = FormStartPosition.CenterScreen;

            // Главная сетка: 2 строки (1-я для картинки, 2-я для панели управления)
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 88F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));

            // Поле для картинки
            pictureControl = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.Fixed3D,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            layoutPaneel.Controls.Add(pictureControl, 0, 0);
            layoutPaneel.SetColumnSpan(pictureControl, 2);

            // Чекбокс масштабирования
            stretchCheck = new CheckBox
            {
                Text = "Venita pilt",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            stretchCheck.CheckedChanged += (s, e) =>
            {
                pictureControl.SizeMode = stretchCheck.Checked
                    ? PictureBoxSizeMode.StretchImage
                    : PictureBoxSizeMode.CenterImage;
            };
            layoutPaneel.Controls.Add(stretchCheck, 0, 1);

            // Панель для кнопок
            buttonPaneel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };

            // Создаем стандартные кнопки
            closeBtn = LooNupp("Sulge", Color.FromArgb(149, 165, 166));
            closeBtn.Click += (s, e) => Close();

            clearBtn = LooNupp("Eemalda pilt", Color.FromArgb(231, 76, 60));
            clearBtn.Click += (s, e) =>
            {
                pictureControl.Image = null;
                pildidTee.Clear();
                praeguneIndeks = -1;
            };

            colorBtn = LooNupp("Muuda tausta", Color.FromArgb(155, 89, 182));
            colorBtn.Click += (s, e) =>
            {
                using (ColorDialog colorDlg = new ColorDialog())
                {
                    if (colorDlg.ShowDialog() == DialogResult.OK)
                        pictureControl.BackColor = colorDlg.Color;
                }
            };

            // Кнопки переключения (< и >)
            eelmineBtn = LooNupp("<", Color.FromArgb(52, 73, 94));
            jargmineBtn = LooNupp(">", Color.FromArgb(52, 73, 94));

            eelmineBtn.Click += (s, e) => KuvaPilt(-1); // Назад
            jargmineBtn.Click += (s, e) => KuvaPilt(1);   // Вперед

            // Кнопка открытия картинки
            showBtn = LooNupp("Ava pilt", Color.FromArgb(52, 152, 219));
            showBtn.Click += (s, e) =>
            {
                using (OpenFileDialog openDlg = new OpenFileDialog())
                {
                    openDlg.Filter = "Pildid|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    if (openDlg.ShowDialog() == DialogResult.OK)
                    {
                        string valitudFail = openDlg.FileName;
                        string kaust = Path.GetDirectoryName(valitudFail);

                        // Получаем все картинки из папки
                        string[] laendid = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        pildidTee = Directory.GetFiles(kaust)
                            .Where(f => laendid.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();

                        praeguneIndeks = pildidTee.IndexOf(valitudFail);
                        pictureControl.Load(valitudFail);
                    }
                }
            };

            // Добавляем кнопки на панель в обратном порядке (из-за RightToLeft)
            buttonPaneel.Controls.Add(closeBtn);
            buttonPaneel.Controls.Add(clearBtn);
            buttonPaneel.Controls.Add(colorBtn);
            buttonPaneel.Controls.Add(showBtn);
            buttonPaneel.Controls.Add(jargmineBtn);
            buttonPaneel.Controls.Add(eelmineBtn);

            layoutPaneel.Controls.Add(buttonPaneel, 1, 1);
            Controls.Add(layoutPaneel);
        }

        // Метод для переключения картинок
        private void KuvaPilt(int samm)
        {
            if (pildidTee == null || pildidTee.Count == 0) return;

            praeguneIndeks += samm;

            // Зацикливание: если дошли до конца — переходим в начало, и наоборот
            if (praeguneIndeks >= pildidTee.Count)
                praeguneIndeks = 0;
            else if (praeguneIndeks < 0)
                praeguneIndeks = pildidTee.Count - 1;

            pictureControl.Load(pildidTee[praeguneIndeks]);
        }

        private Button LooNupp(string tekst, Color varv)
        {
            Button btn = new Button
            {
                Text = tekst,
                AutoSize = true,
                Height = 30,
                BackColor = varv,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(3)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
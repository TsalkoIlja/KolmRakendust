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
        // Vormi väljad graafiliste juhtelementide jaoks
        private PictureBox pictureControl; // Element pildi kuvamiseks
        private CheckBox stretchCheck; // Märkeruut pildi skaleerimiseks/venitamiseks
        private Button showBtn, clearBtn, colorBtn, closeBtn, saveBtn, slideBtn; // Põhinupud
        private Button eelmineBtn, jargmineBtn; // Navigeerimisnupud "Tagasi" ja "Edasi"
        private TableLayoutPanel layoutPaneel; // Peamine tabelipaneel paigutuse jaoks
        private FlowLayoutPanel buttonPaneel; // Paneel nuppude mugavaks ühele reale paigutamiseks
        private Timer slideTimer;

        private List<string> pildidTee = new List<string>(); // Kõigi avatud kaustas olevate piltide teede loend
        private int praeguneIndeks = -1; // Praeguse kuvatava pildi indeks loendis

        public PildivaatjaForm() // Vormi konstruktor
        {
            Text = "Pildi vaatamise programm"; // Akna pealkirja määramine
            Size = new Size(800, 600); // Akna algsed mõõtmed (800x600)
            StartPosition = FormStartPosition.CenterScreen; // Akna kuvamine ekraani keskel

            // Elementide ruudustikutabeli loomine (2 veergu, 2 rida)
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Kogu vormi pinna täitmine
                ColumnCount = 2, // 2 veergu
                RowCount = 2 // 2 rida
            };
            // Veergude laiuse osakaalud (20% ja 80%)
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            // Ridade kõrguse osakaalud (85% pildiala, 15% nupud)
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            // Element pildi kuvamiseks
            pictureControl = new PictureBox
            {
                Dock = DockStyle.Fill, // Kogu lahter täidetakse
                BorderStyle = BorderStyle.Fixed3D, // Mahuline raam
                SizeMode = PictureBoxSizeMode.CenterImage // Vaikimisi pilt keskel ilma moonutusteta
            };
            layoutPaneel.Controls.Add(pictureControl, 0, 0); // Paigutame lahtrisse (0,0)
            layoutPaneel.SetColumnSpan(pictureControl, 2); // Pilt võtab mõlemad veerud

            // Märkeruut (CheckBox) pildi venitamise sisselülitamiseks
            stretchCheck = new CheckBox
            {
                Text = "Venita pilt", // Tekst "Venita pilt"
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            // Pildi kuvamisrežiimi vahetamine linnukese oleku muutmisel
            stretchCheck.CheckedChanged += (s, e) =>
            {
                pictureControl.SizeMode = stretchCheck.Checked
                    ? PictureBoxSizeMode.StretchImage // Kui linnuke on väljas – venitada
                    : PictureBoxSizeMode.CenterImage; // Kui ei – keskele
            };
            layoutPaneel.Controls.Add(stretchCheck, 0, 1); // Lisame 1. rida, 0. veerg

            // Konteiner juhtnuppude horisontaalseks paigutamiseks
            buttonPaneel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft, // Nuppude paigutus paremalt vasakule
                WrapContents = false // Keela nuppude ülekandmine uuele reale
            };

            // Akna sulgemise nupp
            closeBtn = LooNupp("Sulge", Color.FromArgb(149, 165, 166));
            closeBtn.Click += (s, e) => Close(); // Praeguse vormi sulgemine

            // Pildi eemaldamise nupp
            clearBtn = LooNupp("Eemalda", Color.FromArgb(231, 76, 60));
            clearBtn.Click += (s, e) =>
            {
                pictureControl.Image = null; // Eemaldame pildi
                pildidTee.Clear(); // Tühjendame failiteede loendi
                praeguneIndeks = -1; // Lähtestame indeksi
                slideTimer.Stop(); // Peatame slaidiesitluse, kui see töötas
            };

            // Pildi taustavärvi muutmise nupp
            colorBtn = LooNupp("Taustavärv", Color.FromArgb(155, 89, 182));
            colorBtn.Click += (s, e) =>
            {
                using (ColorDialog colorDlg = new ColorDialog()) // Värvi valimise dialoog
                {
                    if (colorDlg.ShowDialog() == DialogResult.OK)
                        pictureControl.BackColor = colorDlg.Color; // Valitud värvi rakendamine PictureBox taustale
                }
            };

            // Praeguse pildi salvestamise nupp
            saveBtn = LooNupp("Salvesta teise formaati", Color.FromArgb(230, 126, 34));
            saveBtn.Click += (s, e) => SalvestaPilt(); // Salvestamise meetodi kutsumine

            // Slaidiesitluse nupp ja taimeri seadistamine
            slideBtn = LooNupp("Slaidishow", Color.FromArgb(241, 196, 15));
            slideTimer = new Timer { Interval = 2000 }; // Piltide vahetamise intervall – 2 sekundit (2000 ms)

            // Järgmisele pildile lülitumine igal taimeri taktil
            slideTimer.Tick += (s, e) => KuvaPilt(1);

            // Slaidiesitluse sisse-/väljalülitamine nupuvajutusega
            slideBtn.Click += (s, e) =>
            {
                if (slideTimer.Enabled) // Kui juba töötab
                {
                    slideTimer.Stop();
                    slideBtn.BackColor = Color.FromArgb(241, 196, 15); // Tagastame nupu kollase värvi
                }
                else if (pildidTee.Count > 0) // Kui on laaditud pilte
                {
                    slideTimer.Start();
                    slideBtn.BackColor = Color.FromArgb(46, 204, 113); // Muudame nupu värvi roheliseks
                }
            };

            // Galerii navigeerimisnupud
            eelmineBtn = LooNupp("<", Color.FromArgb(52, 73, 94));
            jargmineBtn = LooNupp(">", Color.FromArgb(52, 73, 94));
            eelmineBtn.Click += (s, e) => KuvaPilt(-1); // Sammu võrra tagasi
            jargmineBtn.Click += (s, e) => KuvaPilt(1); // Sammu võrra edasi

            // Pildi avamise nupp faili valimise dialoogi kaudu
            showBtn = LooNupp("Ava pilt", Color.FromArgb(52, 152, 219));
            showBtn.Click += (s, e) =>
            {
                using (OpenFileDialog openDlg = new OpenFileDialog()) // Faili avamise dialoog
                {
                    openDlg.Filter = "Pildid|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Faililaiendite filter
                    if (openDlg.ShowDialog() == DialogResult.OK)
                    {
                        string valitudFail = openDlg.FileName; // Valitud faili tee
                        string kaust = Path.GetDirectoryName(valitudFail); // Kaust, kus fail asub

                        // Leiame kõik toetatud graafikafailid sellest samast kaustast
                        string[] laendid = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        pildidTee = Directory.GetFiles(kaust)
                            .Where(f => laendid.Contains(Path.GetExtension(f).ToLower())) // Filtreerimine laiendi järgi
                            .ToList();

                        praeguneIndeks = pildidTee.IndexOf(valitudFail); // Avatud faili indeksi määramine
                        pictureControl.Load(valitudFail); // Pildi laadimine ja kuvamine
                    }
                }
            };

            // Kõigi nuppude lisamine nuppude paneelile
            buttonPaneel.Controls.Add(closeBtn);
            buttonPaneel.Controls.Add(clearBtn);
            buttonPaneel.Controls.Add(saveBtn);
            buttonPaneel.Controls.Add(slideBtn);
            buttonPaneel.Controls.Add(colorBtn);
            buttonPaneel.Controls.Add(showBtn);
            buttonPaneel.Controls.Add(jargmineBtn);
            buttonPaneel.Controls.Add(eelmineBtn);

            layoutPaneel.Controls.Add(buttonPaneel, 1, 1); // Nuppude paneeli lisamine ruudustikku (rida 1, veerg 1)

            Controls.Add(layoutPaneel); // Peamise paigutuse lisamine vormile
        }

        // Meetod praeguse pildi vahetamiseks (edasi/tagasi) ringikujuliselt
        private void KuvaPilt(int samm)
        {
            if (pildidTee == null || pildidTee.Count == 0) return; // Kui loend on tühi – ei tee midagi

            praeguneIndeks += samm; // Muudame praegust indeksit
            if (praeguneIndeks >= pildidTee.Count) praeguneIndeks = 0; // Loendi algusesse minek üle piiri minnes
            else if (praeguneIndeks < 0) praeguneIndeks = pildidTee.Count - 1; // Loendi lõppu minek tagasi liikumisel

            pictureControl.Load(pildidTee[praeguneIndeks]); // Uue faili laadimine
        }

        // Meetod avatud pildi salvestamiseks valitud vormingus
        private void SalvestaPilt()
        {
            if (pictureControl.Image == null) return; // Kontroll, et pilt on laaditud

            using (SaveFileDialog saveDlg = new SaveFileDialog()) // Faili salvestamise dialoog
            {
                saveDlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp"; // Salvestusvormingud
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png; // Vaikimisi vorming – PNG
                    switch (saveDlg.FilterIndex) // Vormingu määramine vastavalt kasutaja valikule
                    {
                        case 2: format = ImageFormat.Jpeg; break; // JPEG
                        case 3: format = ImageFormat.Bmp; break; // BMP
                    }
                    pictureControl.Image.Save(saveDlg.FileName, format); // Faili salvestamine kettale
                }
            }
        }

        // Abimeetod ilusate nuppude ühtseks loomiseks
        private Button LooNupp(string tekst, Color varv)
        {
            Button btn = new Button
            {
                Text = tekst,
                AutoSize = true, // Automaatne suurus vastavalt teksti laiusele
                Height = 30, // Fikseeritud kõrgus
                BackColor = varv,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, // Lame kuvamisstiil
                Font = new Font("Segoe UI", 9, FontStyle.Bold), // Šrift
                Margin = new Padding(2) // Veoruum nupu ümber
            };
            btn.FlatAppearance.BorderSize = 0; // Eemaldame nupu välise raami
            return btn; // Valmis nupu tagastamine
        }
    }
}
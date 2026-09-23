using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace KolmRakendust
{
    public class MalumangForm : Form
    {
        // Vormi privaatsed väljad
        private TableLayoutPanel layoutPaneel; // Tabelipaneel kaartide paigutamiseks
        private List<string> ikoonid = new List<string> // Webdings kirjatüübi sümbolite loend kaartide jaoks (paarid)
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

        private int kulunudAeg = 0; // Kulunud aeg sekundites
        private int kaikudeArv = 0; // Tehtud käikude arv

        // Parimate tulemuste salvestamine seansi jooksul
        private int parimAeg = int.MaxValue;
        private int parimadKaikud = int.MaxValue;

        private Random rand = new Random(); // Juhuslike arvude generaator kaartide segamiseks

        public MalumangForm() // Vormi konstruktor
        {
            Text = "Mälumäng"; // Akna pealkiri
            Size = new Size(600, 680); // Akna mõõtmed
            StartPosition = FormStartPosition.CenterScreen; // Akna kuvamine ekraani keskel
            BackColor = Color.FromArgb(245, 247, 250); // Hele taustavärv

            TableLayoutPanel peaPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Kogu vormi täitmine
                RowCount = 2, // Kaks rida (ülemine paneel ja mänguväli)
                ColumnCount = 1 // Üks veerg
            };
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Ülemise paneeli kõrgus
            peaPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Mänguväli võtab ülejäänud pinna

            // Ülemine paneel juhtelementidega
            FlowLayoutPanel yleminePaneel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            staatusLabel = new Label { Text = "Aeg: 0 s | Käigud: 0", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            // Tekstisilt parima tulemuse jaoks
            parimTulemusLabel = new Label { Text = "Parim: -", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.DarkGreen };

            // Raskusastme rippmenüü
            taseCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            taseCombo.Items.AddRange(new object[] { "2x2 (Kerge)", "4x4 (Tavaline)" }); // Raskusastmed
            taseCombo.SelectedIndex = 1; // Vaikimisi 4x4

            // Uue mängu alustamise nupp
            startNupp = new Button { Text = "Uus mäng", AutoSize = true };
            startNupp.Click += (s, e) => AlustaMangu(); // Klõpsamisel käivitatakse meetod AlustaMangu

            // Elementide lisamine ülemisele paneelile
            yleminePaneel.Controls.Add(taseCombo);
            yleminePaneel.Controls.Add(startNupp);
            yleminePaneel.Controls.Add(staatusLabel);
            yleminePaneel.Controls.Add(parimTulemusLabel);

            peaPaneel.Controls.Add(yleminePaneel, 0, 0); // Ülemise paneeli paigutamine esimesele reale

            // Mänguvälja paneel
            layoutPaneel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            peaPaneel.Controls.Add(layoutPaneel, 0, 1); // Mänguvälja paigutamine teisele reale

            // Taimeri seadistamine kaartide peitmise viivituse jaoks (750 ms)
            taimer = new Timer { Interval = 750 };
            taimer.Tick += (s, e) =>
            {
                taimer.Stop(); // Taimeri peatamine
                esimeneVajutus.ForeColor = esimeneVajutus.BackColor; // Peidame esimese kaardi ikooni
                teineVajutus.ForeColor = teineVajutus.BackColor; // Peidame teise kaardi ikooni
                esimeneVajutus = null; // Esimese kaardi tühistamine
                teineVajutus = null; // Teise kaardi tühistamine
            };

            // Sekunditaimeri seadistamine üldise mänguaja jaoks
            mangTaimer = new Timer { Interval = 1000 };
            mangTaimer.Tick += (s, e) =>
            {
                kulunudAeg++; // Sekundite loenduri suurendamine
                UuendaStaatust(); // Olekuriba uuendamine
            };

            Controls.Add(peaPaneel); // Peakonteineri lisamine vormile
            AlustaMangu(); // Automaatne mängu alustamine avamisel
        }

        // Mängu alustamise / taaskäivitamise meetod
        private void AlustaMangu()
        {
            mangTaimer.Stop();
            kulunudAeg = 0; // Praeguse mängu aja nullimine
            kaikudeArv = 0; // Praeguse mängu käikude nullimine
            UuendaStaatust(); // Oleku uuendamine ekraanil
            esimeneVajutus = null;
            teineVajutus = null;

            // Eelmise ruudustiku puhastamine
            layoutPaneel.Controls.Clear();
            layoutPaneel.ColumnStyles.Clear();
            layoutPaneel.RowStyles.Clear();

            // Ruudustiku suuruse määramine (2x2 või 4x4)
            int suurus = taseCombo.SelectedIndex == 0 ? 2 : 4;
            layoutPaneel.ColumnCount = suurus;
            layoutPaneel.RowCount = suurus;

            // Veergude ja ridade proportsionaalne seadistamine
            for (int i = 0; i < suurus; i++)
            {
                layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / suurus));
                layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / suurus));
            }

            int kokku = suurus * suurus; // Kaartide üldarv
            List<string> kasutatavad = ikoonid.GetRange(0, kokku);
            List<string> koopia = new List<string>(kasutatavad);

            // Kaartide genereerimine ja ruudustikku paigutamine
            for (int i = 0; i < kokku; i++)
            {
                int idx = rand.Next(koopia.Count); // Juhusliku indeksi valimine
                string ikoon = koopia[idx]; // Ikooni saamine
                koopia.RemoveAt(idx); // Valitud ikooni eemaldamine saadaolevate hulgast

                // Kaardi elemendi (Label) loomine
                Label kaart = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = ikoon, // Webdings kirjatüübi sümbol
                    Font = new Font("Webdings", suurus == 2 ? 60 : 36, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(155, 89, 182),
                    ForeColor = Color.FromArgb(155, 89, 182), // Tekst on peidetud
                    Margin = new Padding(3),
                    BorderStyle = BorderStyle.FixedSingle
                };

                kaart.Click += Kaart_Click; // Klõpsamise sündmuse sidumine
                layoutPaneel.Controls.Add(kaart); // Kaardi lisamine ruudustikku
            }

            mangTaimer.Start(); // Aja lugemise käivitamine
        }

        // Kaardile vajutamise sündmuse töötleja
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
            kaikudeArv++; // Käikude loenduri suurendamine
            UuendaStaatust(); // Oleku uuendamine

            if (esimeneVajutus.Text == teineVajutus.Text)
            {
                esimeneVajutus = null;
                teineVajutus = null;
                KontrolliVoitu(); // Võidutingimuse kontrollimine
            }
            else
            {
                taimer.Start();
            }
        }

        // Olekuriba uuendamine
        private void UuendaStaatust()
        {
            staatusLabel.Text = $"Aeg: {kulunudAeg} s | Käigud: {kaikudeArv}";
        }

        // Võidutingimuse kontroll ja parima tulemuse uuendamine
        private void KontrolliVoitu()
        {
            foreach (Control control in layoutPaneel.Controls)
            {
                Label kaart = control as Label;
                if (kaart != null && kaart.ForeColor == kaart.BackColor)
                    return;
            }

            mangTaimer.Stop(); // Aja peatamine pärast võitu

            // Parima tulemuse kontrollimine ja salvestamine
            if (kulunudAeg < parimAeg)
            {
                parimAeg = kulunudAeg;
                parimadKaikud = kaikudeArv;
                parimTulemusLabel.Text = $"Parim: {parimAeg} s ({parimadKaikud} käiku)";
            }

            // Dialoogiakna kuvamine võidutulemustega
            MessageBox.Show($"Võit!\r\nPraegune aeg: {kulunudAeg} s, käike: {kaikudeArv}\r\nParim tulemus: {parimAeg} s", "Palju õnne!");
        }
    }
}
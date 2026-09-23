using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace KolmRakendust
{
    public class MatemaatikaForm : Form
    {
        // Väljad matemaatiliste tehete siltide jaoks
        private Label addLabel, subLabel, multLabel, divLabel;
        // Väljad infosiltide jaoks (tulemuse teade, taimer, punktide loendur)
        private Label tulemusLabel, aegLabel, punktidLabel;
        // Sisestusväljad kasutaja vastuste jaoks
        private NumericUpDown addInput, subInput, multInput, divInput;
        // Nupud mängu alustamiseks ja enneaegseks lõpetamiseks
        private Button alustaNupp, lopetaNupp;
        // Rippmenüü raskusastme valimiseks
        private ComboBox raskusasteCombo;
        // Taimer aja tagasiarvestuseks
        private Timer taimer;

        // Muutujad igapäevaste tehete arvude (operandide) jaoks
        private int addA, addB, subA, subB, multA, multB, divA, divB;
        private int jaanudAega;
        private int punktid = 0;
        private Random rand = new Random();
        private TableLayoutPanel layoutPaneel;

        public MatemaatikaForm()
        {
            Text = "Matemaatiline test";
            Size = new Size(550, 630); // Akna mõõtmed
            StartPosition = FormStartPosition.CenterScreen; // Akna kuvamine ekraani keskel
            BackColor = Color.FromArgb(245, 247, 250);

            // Paigutustabeli reastuse algseadistamine (2 veergu, 8 rida)
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(15)
            };

            // Veergude laiuse jagamine: kumbki veerg võtab 50%
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutPaneel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Ridade kõrguste seadistamine
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 0: Raskusaste ja Taimer
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 1: +
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 2: -
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 3: *
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 4: /
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 5: Alusta nupp ja Punktid
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 6: Nupp "Lõpeta"
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rida 7: Tulemus

            // Silt aja kuvamiseks
            aegLabel = new Label
            {
                Text = "Aeg: 40",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            // Silt punktide kuvamiseks
            punktidLabel = new Label
            {
                Text = "Punktid: 0",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185)
            };

            // Rippmenüü raskusastmete valikuga
            raskusasteCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.None,
                Font = new Font("Segoe UI", 10)
            };
            raskusasteCombo.Items.AddRange(new object[] { "Kerge", "Keskmine", "Raske" });
            raskusasteCombo.SelectedIndex = 0;

            // Tekstisiltide loomine tehete jaoks abimeetodi abil
            addLabel = LooLabel("? + ? =");
            subLabel = LooLabel("? - ? =");
            multLabel = LooLabel("? × ? =");
            divLabel = LooLabel("? ÷ ? =");

            // Sisestusväljade loomine abimeetodi abil
            addInput = LooInput();
            subInput = LooInput();
            multInput = LooInput();
            divInput = LooInput();

            MuudaSisenditeOlekut(false);

            // Alusta nupu loomine ja kujundamine
            alustaNupp = new Button
            {
                Text = "Alusta mängu",
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.None
            };
            alustaNupp.FlatAppearance.BorderSize = 0;

            // Nupp mängu enneaegseks lõpetamiseks
            lopetaNupp = new Button
            {
                Text = "Lõpeta",
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(230, 126, 34),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.None,
                Enabled = false // Alguses lukustatud
            };
            lopetaNupp.FlatAppearance.BorderSize = 0;

            // Silt võidu või kaotuse teate kuvamiseks
            tulemusLabel = new Label
            {
                Text = "",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            taimer = new Timer { Interval = 1000 }; // Taimeri intervall 1 sekund (1000 ms)

            // Ülesannete genereerimine sõltuvalt valitud raskusastmest
            alustaNupp.Click += (s, e) =>
            {
                int maxNum = 20;
                jaanudAega = 40;

                // Parameetrite seadistamine vastavalt raskusastmele
                if (raskusasteCombo.SelectedIndex == 1) { maxNum = 50; jaanudAega = 30; }
                else if (raskusasteCombo.SelectedIndex == 2) { maxNum = 100; jaanudAega = 20; }

                // Avame sisestusväljad ja nupu "Lõpeta"
                MuudaSisenditeOlekut(true);
                raskusasteCombo.Enabled = false;
                lopetaNupp.Enabled = true;

                // Taastame väljade taustavärvi
                SebraVarvid(Color.White);

                addA = rand.Next(1, maxNum);
                addB = rand.Next(1, maxNum);
                addLabel.Text = $"{addA} + {addB} =";
                addInput.Value = 0;

                subA = rand.Next(2, maxNum);
                subB = rand.Next(1, subA); // Garanteerime positiivse tulemuse lahutamisel
                subLabel.Text = $"{subA} - {subB} =";
                subInput.Value = 0;

                multA = rand.Next(2, maxNum / 2);
                multB = rand.Next(2, 10);
                multLabel.Text = $"{multA} × {multB} =";
                multInput.Value = 0;

                // Jagamistehte jaoks genereerime arvud nii, et tulemus oleks täisarv
                divB = rand.Next(2, 10);
                int jagaja = rand.Next(1, maxNum / 2);
                divA = divB * jagaja;
                divLabel.Text = $"{divA} ÷ {divB} =";
                divInput.Value = 0;

                aegLabel.Text = $"Aeg: {jaanudAega}";
                tulemusLabel.Text = "";
                taimer.Start();
            };

            // Mängu enneaegne lõpetamine nupule "Lõpeta" vajutamisel
            lopetaNupp.Click += (s, e) =>
            {
                if (!taimer.Enabled) return;

                taimer.Stop();

                bool koikOiged = (addInput.Value == addA + addB) &&
                                 (subInput.Value == subA - subB) &&
                                 (multInput.Value == multA * multB) &&
                                 (divInput.Value == divA / divB);

                if (koikOiged)
                {
                    punktid += 10 + jaanudAega; // Boonuspunktid ülejäänud sekundite eest
                    punktidLabel.Text = $"Punktid: {punktid}";
                    tulemusLabel.Text = "Tubli! Kõik vastused olid õiged!";
                    tulemusLabel.ForeColor = Color.FromArgb(39, 174, 96);
                    SebraVarvid(Color.LightGreen);
                }
                else
                {
                    tulemusLabel.Text = "Mõned vastused olid valed!";
                    tulemusLabel.ForeColor = Color.FromArgb(192, 57, 43);
                    KontrolliJaVarviVastused();
                }

                MuudaSisenditeOlekut(false);
                raskusasteCombo.Enabled = true;
                lopetaNupp.Enabled = false;
            };

            // Vastuste automaatne kontrollimine sisestamisel
            EventHandler kontrolliVastuseid = (s, e) =>
            {
                if (taimer.Enabled &&
                    addInput.Value == addA + addB &&
                    subInput.Value == subA - subB &&
                    multInput.Value == multA * multB &&
                    divInput.Value == divA / divB)
                {
                    taimer.Stop();
                    punktid += 10 + jaanudAega; // Boonuspunktid ülejäänud aja eest
                    punktidLabel.Text = $"Punktid: {punktid}";
                    tulemusLabel.Text = "Õige! Kõik ülesanded lahendatud!";
                    tulemusLabel.ForeColor = Color.FromArgb(39, 174, 96);

                    SebraVarvid(Color.FromArgb(212, 239, 223));
                    raskusasteCombo.Enabled = true;
                    lopetaNupp.Enabled = false;
                }
            };

            addInput.ValueChanged += kontrolliVastuseid;
            subInput.ValueChanged += kontrolliVastuseid;
            multInput.ValueChanged += kontrolliVastuseid;
            divInput.ValueChanged += kontrolliVastuseid;

            // Aja lugemine
            taimer.Tick += (s, e) =>
            {
                jaanudAega--;
                aegLabel.Text = $"Aeg: {jaanudAega}";
                if (jaanudAega <= 0)
                {
                    taimer.Stop();
                    tulemusLabel.Text = "Aeg sai otsa!";
                    tulemusLabel.ForeColor = Color.FromArgb(192, 57, 43);

                    KontrolliJaVarviVastused();

                    // Lukustame sisestusväljad pärast aja lõppemist
                    MuudaSisenditeOlekut(false);
                    raskusasteCombo.Enabled = true;
                    lopetaNupp.Enabled = false;
                }
            };

            // Elementide paigutamine tabelipaneeli
            layoutPaneel.Controls.Add(raskusasteCombo, 0, 0);
            layoutPaneel.Controls.Add(aegLabel, 1, 0);

            // Tehete ridade lisamine
            LisaRida(addLabel, addInput, 1);
            LisaRida(subLabel, subInput, 2);
            LisaRida(multLabel, multInput, 3);
            LisaRida(divLabel, divInput, 4);

            // Alusta nupu ja punktide paigutamine
            layoutPaneel.Controls.Add(alustaNupp, 0, 5);
            layoutPaneel.Controls.Add(punktidLabel, 1, 5);

            // Enneaegse lõpetamise nupu paigutamine
            layoutPaneel.Controls.Add(lopetaNupp, 0, 6);
            layoutPaneel.SetColumnSpan(lopetaNupp, 2);

            // Tulemuse oleku paigutamine
            layoutPaneel.Controls.Add(tulemusLabel, 0, 7);
            layoutPaneel.SetColumnSpan(tulemusLabel, 2);

            Controls.Add(layoutPaneel);
        }

        // Abimeetod kõigi sisestusväljade sisse-/väljalülitamiseks
        private void MuudaSisenditeOlekut(bool olek)
        {
            addInput.Enabled = olek;
            subInput.Enabled = olek;
            multInput.Enabled = olek;
            divInput.Enabled = olek;
        }

        // Õigete ja valede vastuste värvimine
        private void KontrolliJaVarviVastused()
        {
            addInput.BackColor = (addInput.Value == addA + addB) ? Color.LightGreen : Color.LightCoral;
            subInput.BackColor = (subInput.Value == subA - subB) ? Color.LightGreen : Color.LightCoral;
            multInput.BackColor = (multInput.Value == multA * multB) ? Color.LightGreen : Color.LightCoral;
            divInput.BackColor = (divInput.Value == divA / divB) ? Color.LightGreen : Color.LightCoral;
        }

        // Väljadele ühtlase taustavärvi määramine
        private void SebraVarvid(Color color)
        {
            addInput.BackColor = color;
            subInput.BackColor = color;
            multInput.BackColor = color;
            divInput.BackColor = color;
        }

        // Abimeetod ühesuguste tekstisiltide loomiseks
        private Label LooLabel(string tekst)
        {
            return new Label
            {
                Text = tekst,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Anchor = AnchorStyles.Right
            };
        }

        // Abimeetod arvu sisestusväljade loomiseks
        private NumericUpDown LooInput()
        {
            NumericUpDown input = new NumericUpDown
            {
                Width = 90,
                Font = new Font("Segoe UI", 11),
                Maximum = 10000,
                Anchor = AnchorStyles.Left
            };

            input.Enter += (s, e) => input.Select(0, input.Text.Length);
            return input;
        }

        // Abimeetod paari "Silt + Sisestusväli" lisamiseks tabeli reale
        private void LisaRida(Label lbl, NumericUpDown inp, int row)
        {
            layoutPaneel.Controls.Add(lbl, 0, row);
            layoutPaneel.Controls.Add(inp, 1, row);
        }
    }
}
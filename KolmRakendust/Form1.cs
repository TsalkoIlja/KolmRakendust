using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public partial class Form1 : Form
    {
        // Privaatsete väljade (muutujate) deklareerimine juhtelementide jaoks
        private Button piltNupp;
        private Button matemaatikaNupp;
        private Button malumangNupp;
        private TableLayoutPanel layoutPaneel;

        public Form1()
        {
            Text = "Peamenüü - Kolm Rakendust"; // Akna pealkirja määramine
            Size = new Size(550, 500); // Akna suuruse määramine (laius: 550, kõrgus: 500 pikslit)
            StartPosition = FormStartPosition.CenterScreen; // Akna kuvamine ekraani keskel käivitamisel
            BackColor = Color.FromArgb(245, 247, 250); // Vormi heleda taustavärvi määramine RGB-koodiga

            // Tabelipaneeli (TableLayoutPanel) loomine nuppude automaatseks joondamiseks
            layoutPaneel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Paneeli laotamine üle kogu vormi pinna
                ColumnCount = 1, // Veergude arvu määramine (1 veerg)
                RowCount = 3, // Ridade arvu määramine (3 rida)
                Padding = new Padding(20) // Sise-ääris paneeli servadest 20 pikslit
            };

            // Ridade kõrguse seadistamine: iga rida võtab 33.3% paneeli kõrgusest
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));
            layoutPaneel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));

            // Kolme nupu loomine ja seadistamine abimeetodi LooKenaNupp abil
            piltNupp = LooKenaNupp("Pildi vaatamise programm", Color.FromArgb(52, 152, 219));
            matemaatikaNupp = LooKenaNupp("Matemaatiline mäng", Color.FromArgb(46, 204, 113));
            malumangNupp = LooKenaNupp("Mälumäng", Color.FromArgb(155, 89, 182));

            // Nupu vajutamise sündmuste sidumine (klõpsamisel luuakse ja avatakse vastav vorm)
            piltNupp.Click += (s, e) => new PildivaatjaForm().Show();
            matemaatikaNupp.Click += (s, e) => new MatemaatikaForm().Show();
            malumangNupp.Click += (s, e) => new MalumangForm().Show();

            // Loodud nuppude lisamine tabelipaneeli lahtritesse
            layoutPaneel.Controls.Add(piltNupp, 0, 0);
            layoutPaneel.Controls.Add(matemaatikaNupp, 0, 1);
            layoutPaneel.Controls.Add(malumangNupp, 0, 2);

            Controls.Add(layoutPaneel); // Kujundatud paneeli lisamine peavormile
        }

        // Abimeetod nuppude loomiseks ja visuaalseks kujundamiseks
        private Button LooKenaNupp(string tekst, Color taustaVarv)
        {
            var nupp = new Button // Uue nupu objekti loomine
            {
                Text = tekst, // Nupu teksti määramine
                Size = new Size(280, 55), // Nupu standardsuuruse määramine (280x55)
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), // Kirjatüübi seadistamine: Segoe UI, suurus 11, rasvane
                ForeColor = Color.White,
                BackColor = taustaVarv, // Taustavärv — parameetrina edastatud värv
                FlatStyle = FlatStyle.Flat, // Nupu lame stiil (ilma 3D-raamita)
                Anchor = AnchorStyles.None // Servade külge kinnitamise tühistamine, et nupp jääks lahtri keskele
            };

            nupp.FlatAppearance.BorderSize = 0; // Välise äärisjoone eemaldamine nupu ümbert

            return nupp; // Täielikult seadistatud nupu tagastamine
        }
    }
}

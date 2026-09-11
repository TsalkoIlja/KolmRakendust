# Kolm Rakendust (Windows Forms)

## Projekti nimi ja kirjeldus
Projekt "Kolm Rakendust" on C# Windows Forms keskkonnas loodud õppeprojekt, mis koosneb kolmest eraldiseisvast rakendusest ja peamenüüst. Kõik kasutajaliidese (GUI) elemendid on loodud dünaamiliselt koodi kaudu (ilma Visual Studio Designer / Toolboxi kasutamata), järgides objektorienteeritud programmeerimise (OOP) põhimõtteid.

## Kasutatud tehnoloogiad
* **Keel:** C# (.NET)
* **Raamistik:** Windows Forms (WinForms)
* **Arenduskeskkond:** Visual Studio 2022

## Kuidas programmi käivitada ja kasutada
1. Ava projekt Visual Studio 2022 keskkonnas.
2. Käivita programm (`F5` või `Start` nupp).
3. Avaneb peamenüü (`Form1`), kust saab valida ühe kolmest rakendusest:
   * **Pildi vaatamise programm:** Ava pilte, muuda taustavärvi, eemalda pilt või skaala seda.
   * **Matemaatiline test:** Lahenda 4 erinevat tehet (liitmine, lahutamine, korrutamine, jagamine) enne aja lõppemist (30 sekundit).
   * **Mälumäng:** Otsi 4x4 kaardivõrgustikust sarnaseid sümbolipaare.

---

## Projektide edasiarenduse ideed (Eeldav edasi areng)

### 1. Pildi vaatamise programm (PildivaatjaForm)
* **Slaidishow režiim:** Automaatne piltide vahetamine valitud kaustast määrama aja tagant (kasutades `Timer` elementi).
* **Pildi töötlemine ja filtrid:** Võimalus muuta pilt must-valgeks, pöörata pilti või rakendada muid lihtsaid filtreid.
* **Salvestamine teise formaati:** Nupp pildi salvestamiseks soovitud formaati (nt PNG, JPG) kasutades `SaveFileDialog` dialoogi.

### 2. Matemaatiline test (MatemaatikaForm)
* **Raskusastmete valik:** Kerge (numbrid 1–10), Keskmine (1–50) ja Raske (1–100 või ujukomaarvud).
* **Punktisüsteem ja edetabel:** Parimate tulemuste (aeg + õiged vastused) salvestamine kohalikku faili.
* **Heli-efektid:** Helisignaalid õige vastuse, vale vastuse ja aja lõppemise korral.

### 3. Mälumäng (MalumangForm)
* **Tegelikud pildid sümbolite asemel:** `Webdings` kirjatüübi sümbolite asendamine tegelike pildifailidega (PNG/JPG).
* **Mänguvälja suuruse valik (Tasemed):** Võimalus valida erinevaid ruudustikke (nt 2x2 algajatele, 4x4 tavaline, 6x6 eksperdile).
* **Käikude loendur ja taimer:** Punktide arvutamine vastavalt sellele, kui väheste käikudega ja kui kiiresti kõik paarid leitakse.
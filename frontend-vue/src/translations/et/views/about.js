export default {
  section: [
    {
      header: {
        h2: "Test ülesande kirjeldus (esialgne)",
      },
      p_1: "Ülesanne on teostada lihtne veebirakendus, mis võimaldaks hallata kasutajatoele saadetud pöördumisi. Lihtsustatud süsteemi funktsionaalsus oleks järgmine.",
      ol: {
        li: [
          "Kasutaja saab sisestada pöördumis.",
          "Pöördumisel peab olema kirjeldus, sisestamise aeg, lahendamise tähtaeg. Sisestamise ajaks märgitakse pöördumise sisestamise aeg, teised kohustuslikud väljad täidab kasutaja.",
          "Kasutajale kuvatakse aktiivsed pöördumised koos kõigi väljadega  nimekirjas sorteeritult kahanevalt lahendamise tähtaja järgi.",
          "Pöördumised, mille lahendamise tähtajani on jäänud vähem kui 1 tund või mis on juba ületanud lahendamise tähtaja, kuvatakse nimekirjas punasena.",
          "Kasutaja saab nimekirjas pöördumisi lahendatuks märkida, mis kaotab pöördumise nimekirjast.",
        ],
      },
      p_2: "Võib eeldada modernse brauseri olemasolu (HTML5 jne). Andmeid ei ole vaja andmebaasi salvestada, võib vabalt hoida neid ka mälus.Sooviksime töös näha kindlasti ka üksusteste.",
    },
    {
      header: {
        h2: "Lisanduv funktsionaalsus",
      },
      p_1: "Otsistasin seda projekti edasi arendada, et eksperimenteerida ASP.NET Core Web API, VueJS ja teiste seotud tehnoloogiatega.",
      section: [
        {
          header: {
            h3: "Seisuga 02. veebruar 2022:",
          },
          ol: {
            li: [
              "Vue osad on refaktooritud Composition API-le.",
              "Katsetan tekstide tõlgetega, mitmekeelsuse tugi. Kirjutasin pulgina, mis laeb tekste erinevates keeltes kõikidele vaadetele ja komponentidele, kus esineb mingisugust nähtavat teksti.",
            ],
          },
        },
        {
          header: {
            h3: "Seisuga 16. veebruar 2022:",
          },
          h4_1: "Keele haldusega seotud muudatused:",
          ol_1: {
            li: [
              "Esmasel app-i avamisel võrreldakse, kas browser-i peamine keel on toetatud keelte hulgas. Kui on, siis pannakse see app-i keeleks, muul juhul kasutatakse app-i vaikekeelt.",
              "Keele vahetusel muudetakse <html lang> attribuudi väärtus.",
              "Keele vahetus salvestatakse `window.localStorage` objekti.",
              "Keele vahetusel muudetakse dokumendi tiitel.",
              "Keele vahetus muudab kuupäevade vormingud ka API-st päritud info kuvamisl, kuid uut API pöördumist ei sooritata.",
            ],
          },
          h4_2: "Vaade 'Rakendusest' (käesolev) kasutab uut `Tree` komponenti:",
          ol_2: {
            li: [
              "Tree loeb JSON struktuuris HTML struktuuriga info ja renderdab selle vaatesse.",
              "See lähteinfo peab sisaldama HTML struktuuri, kus omadused on korrektsed HTML viibad.",
              "Toetatud on nii Javascript Array, Object ja String väärtused.",
            ],
          },
        },
      ],
    },
  ],
};

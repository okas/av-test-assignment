export default {
  section: [
    {
      header: {
        h1: "Test ülesande kirjeldus (esialgne)",
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
        h1: "Lisanduv funktsionaalsus",
      },
      p_1: "Otsistasin seda projekti edasi arendada, et eksperimenteerida ASP.NET Core Web API, VueJS ja teiste seotud tehnoloogiatega.",
      p_2: "Seisuga 02. veebruar 2022:",
      ol: {
        li: [
          "Vue osad on refaktooritud Composition API-le.",
          "Katsetan tekstide tõlgetega, mitmekeelsuse tugi. Kirjutasin pulgina, mis laeb tekste erinevates keeltes kõikidele vaadetele ja komponentidele, kus esineb mingisugust nähtavat teksti.",
        ],
      },
    },
  ],
};

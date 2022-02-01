export default {
  // TODO Might tot be appropriate for sharing, but to store excercising it's OK. Get it from API, and fetch it from components/views directly.
  en: {
    header: "Description of test assignment (original)",
    p1: "The task is to implement a simple web application that allows to manage the requests sent to the support. The functionality of the simplified system would be as follows.",
    ol_items: [
      "The user can enter a request,",
      "The request must contain a description, creation time, the deadline time for resolution; the creation time must be timestamp by system, the other mandatory fields are filled in by the user,",
      "The user is shown active requests with all fields sorted in descending order by resolution deadline.",
      "Requests that are less than 1 hour away or have already expired will be listed in red in the list.",
      "The user can mark requests resolved in the list, which removes it from the list.",
    ],
    p2: "A modern browser (HTML5, etc.) can be assumed. It is not necessary to store the data in the database, you can freely keep it in the memory as well. We would definitely like to see unit tests at work as well.",
  },
  et: {
    header: "Test ülesande kirjeldus (esialgne)",
    p1: "Ülesanne on teostada lihtne veebirakendus, mis võimaldaks hallata kasutajatoele saadetud pöördumisi. Lihtsustatud süsteemi funktsionaalsus oleks järgmine.",
    ol_items: [
      "Kasutaja saab sisestada pöördumise,",
      "Pöördumisel peab olema kirjeldus, sisestamise aeg, lahendamise tähtaeg. Sisestamise ajaks märgitakse pöördumise sisestamise aeg, teised kohustuslikud väljad täidab kasutaja.",
      "Kasutajale kuvatakse aktiivsed pöördumised koos kõigi väljadega  nimekirjas sorteeritult kahanevalt lahendamise tähtaja järgi.",
      "Pöördumised, mille lahendamise tähtajani on jäänud vähem kui 1 tund või mis on juba ületanud lahendamise tähtaja, kuvatakse nimekirjas punasena.",
      "Kasutaja saab nimekirjas pöördumisi lahendatuks märkida, mis kaotab pöördumise nimekirjast.",
    ],
    p2: "Võib eeldada modernse brauseri olemasolu (HTML5 jne). Andmeid ei ole vaja andmebaasi salvestada, võib vabalt hoida neid ka mälus.Sooviksime töös näha kindlasti ka üksusteste.",
  }
}

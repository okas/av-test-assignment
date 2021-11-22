<template>
  <div class="about">
    <h1>Test ülesande kirjeldus</h1>
    <p>
      Ülesanne on teostada lihtne veebirakendus, mis võimaldaks hallata
      kasutajatoele saadetud pöördumisi. Lihtsustatud süsteemi funktsionaalsus
      oleks järgmine:
    </p>
    <ol>
      <li>Kasutaja saab sisestada pöördumise.</li>
      <li>
        Pöördumisel peab olema kirjeldus, sisestamise aeg, lahendamise tähtaeg.
        Sisestamise ajaks märgitakse pöördumise sisestamise aeg, teised
        kohustuslikud väljad täidab kasutaja.
      </li>
      <li>
        Kasutajale kuvatakse aktiivsed pöördumised koos kõigi väljadega
        nimekirjas sorteeritult kahanevalt lahendamise tähtaja järgi.
      </li>
      <li>
        Pöördumised, mille lahendamise tähtajani on jäänud vähem kui 1 tund või
        mis on juba ületanud lahendamise tähtaja, kuvatakse nimekirjas punasena.
      </li>
      <li>
        Kasutaja saab nimekirjas pöördumisi lahendatuks märkida, mis kaotab
        pöördumise nimekirjast.
      </li>
    </ol>
    <p>
      Võib eeldada modernse brauseri olemasolu (HTML5 jne). Andmeid ei ole vaja
      andmebaasi salvestada, võib vabalt hoida neid ka mälus. Sooviksime töös
      näha kindlasti ka üksusteste.
    </p>
  </div>
</template>

<style scoped>
.about {
  text-align: justify;
}
</style>

<template>
  <div class="hello">
    <h1>'Hello world' leht</h1>
    <p>
      Kiire indikatsioon, et Web API käivitunud.
    </p>
    <h3>Ilmaprognoosi info <sup>(API genereeritud)</sup></h3>
    <table>
      <thead>
        <tr>
          <th>Kuupäev</th>
          <th>Temperatuur °C</th>
          <th>Temperatuur °F</th>
          <th>Kirjeldus</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in data" :key="i">
          <td class="item" v-text="formatDate(item.date)" />
          <td class="item" v-text="item.temperatureC" />
          <td class="item" v-text="item.temperatureF" />
          <td class="item" v-text="item.summary" />
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
  import formatDateMixin from '../mixins/formatDateMixin';

  export default {
    name: "WeatherForecast",
    mixins: [formatDateMixin],
    data() {
      return { data: [] }
    },
    beforeMount() {
      this.getForecast();
    },
    methods: {
      async getForecast() {
        const res = await fetch('/weatherforecast');
        this.data = await res.json();
      }
    }
  };
</script>

<style scoped>
  h3 {
    margin: 40px 0 0;
  }

  table {
    margin-left: auto;
    margin-right: auto;
  }

  td {
    color: #42b983;
  }

  sup {
    font-size: 0.5em
  }
</style>

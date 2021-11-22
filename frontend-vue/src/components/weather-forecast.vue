<template>
  <div class="weather-forecast">
    <h2>Ilmaprognoosi info <sup>(API genereeritud)</sup></h2>
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
        <tr v-for="(item, i) in forecasts" :key="i">
          <td class="item">{{ formatDate(item.date) }}</td>
          <td class="item">{{ item.temperatureC }}</td>
          <td class="item">{{ item.temperatureF }}</td>
          <td class="item">{{ item.summary }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import formatDateMixin from "../mixins/formatDateMixin";

export default {
  name: "WeatherForecast",
  mixins: [formatDateMixin],
  data: () => ({ forecasts: [] }),
  beforeMount() {
    this.getForecast();
  },
  methods: {
    async getForecast() {
      const res = await fetch("/weatherforecast");
      this.forecasts = await res.json();
    },
  },
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
  font-size: 0.5em;
}
</style>

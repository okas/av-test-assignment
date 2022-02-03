<template>
  <section class="weather-forecast">
    <h2>{{ translationVm.header[0] }} <sup>{{ translationVm.header[1] }}</sup></h2>
    <table>
      <thead>
        <tr>
          <th v-for="(item, i) in translationVm.tableHeader" :key="i">{{ item }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in forecasts" :key="i">
          <td class="item">{{ formatDateShort(item.date) }}</td>
          <td class="item">{{ item.temperatureC }}</td>
          <td class="item">{{ item.temperatureF }}</td>
          <td class="item">{{ item.summary }}</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script>
import { ref } from "vue";
import formatDateMixin from "../mixins/formatDateMixin";
import { useTranslator } from "../plugins/translatorPlugin";

export default {
  name: "WeatherForecast",
  mixins: [formatDateMixin],
  data: () => ({ forecasts: [] }),
  setup() {
    const translationVm = ref({ header: [], tableHeader: [] });
    useTranslator()("components/weather-forecast").then(data => translationVm.value = data);
    return { translationVm };
  },
  created() {
    this.getForecast();
  },
  methods: {
    async getForecast() {
      const resp = await this.$api.then((client) =>
        client.execute({ operationId: "get_weatherforecast" })
      );
      if (resp.ok) {
        this.forecasts = resp.body;
      }
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

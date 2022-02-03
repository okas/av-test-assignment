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

<script setup>
import { ref } from "vue";
import useFormatDateTime from "../utils/formatDateTime";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import { useTranslator } from "../plugins/translatorPlugin";

const translationVm = ref({ header: [], tableHeader: [] });
const forecasts = ref([]);
const api = useApiClient();
const { formatDateShort } = useFormatDateTime();

useTranslator()("components/weather-forecast").then(data => translationVm.value = data);

api.then(client =>
  client.execute({ operationId: "get_weatherforecast" })
).then(resp => {
  if (resp.ok) forecasts.value = resp.body;
});
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

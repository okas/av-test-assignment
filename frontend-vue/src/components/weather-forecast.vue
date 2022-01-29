<template>
  <section class="weather-forecast">
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
import { ref, onBeforeMount } from "vue";
import useFormatDateTime from "../utils/formatDateTime";
import { useApiClient } from "../plugins/swaggerClientPlugin";

onBeforeMount(getForecast);

const forecasts = ref([]);
const api = useApiClient();
const { formatDateShort } = useFormatDateTime(); // TODO can move to outter scope?
  
async function getForecast() {
  const resp = await api.then((client) =>
    client.execute({ operationId: "get_weatherforecast" })
  );
  if (resp.ok) {
    forecasts.value = resp.body;
  }
}
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

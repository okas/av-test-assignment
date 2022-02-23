<script setup>
import { ref, watchEffect } from "vue";
import useFormatDateTime from "../utils/formatDateTime";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const store = useRootStore();
const forecasts = ref([
  {
    date: 0,
    temperatureC: 0,
    temperatureF: 0,
    summary: "",
  },
]);
const translationVm = ref({ header: [], tableHeader: [] });

const { formatDateShort } = useFormatDateTime();
const translatorAsync = useTranslator();
const api = useApiClient();

watchEffect(
  async () =>
    (translationVm.value = await translatorAsync(
      "components/weather-forecast",
      store.language
    ))
);

const resp = await api.then((client) =>
  client.execute({ operationId: "get_weatherforecast" })
);
if (resp.ok) {
  forecasts.value = resp.obj;
}
</script>

<template>
  <section class="weather-forecast">
    <h2>
      {{ translationVm.header[0] }}<sup v-text="translationVm.header[1]" />
    </h2>
    <table>
      <thead>
        <tr>
          <th
            v-for="(item, i) in translationVm.tableHeader"
            :key="i"
            v-text="item"
          />
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(
            { date, temperatureC, temperatureF, summary }, i
          ) in forecasts"
          :key="i"
        >
          <td class="item" v-text="formatDateShort(date, store.language)" />
          <td class="item" v-text="temperatureC" />
          <td class="item" v-text="temperatureF" />
          <td class="item" v-text="summary" />
        </tr>
      </tbody>
    </table>
  </section>
</template>

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

<template>
  <article>
    <img alt="Vue logo" src="../assets/logo.png" />
    <header>
      <h1>{{ translationVm.header }}</h1>
    </header>
    <p>{{ translationVm.p1 }}</p>
    <Suspense>
      <template #default>
        <WeatherForecast />
      </template>
      <template #fallback>
        <div style="color: red; font-weight: 900; font-size: larger">
          # loading... #
        </div>
      </template>
    </Suspense>
  </article>
</template>

<script setup>
import { ref, watchEffect } from "vue";
import WeatherForecast from "../components/weather-forecast.vue";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const store = useRootStore();
const translationVm = ref({
  header: "",
  p1: "",
});

const translatorAsync = useTranslator();

watchEffect(
  async () =>
    (translationVm.value = await translatorAsync("views/Home", store.language))
);
</script>

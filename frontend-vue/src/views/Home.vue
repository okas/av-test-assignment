<script setup>
import { defineAsyncComponent, ref, watchEffect } from "vue";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const WeatherForecast = defineAsyncComponent(() =>
  import("../components/weather-forecast.vue")
);

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

<template>
  <article>
    <img alt="Vue logo" src="../assets/logo.png" />
    <header>
      <h1 v-text="translationVm.header" />
    </header>
    <p v-text="translationVm.p1" />
    <Suspense>
      <template #default>
        <WeatherForecast />
      </template>
      <template #fallback>
        <!--  ToDo: To reusable component -->
        <div style="color: red; font-weight: 900; font-size: larger">
          # loading... #
        </div>
      </template>
    </Suspense>
  </article>
</template>

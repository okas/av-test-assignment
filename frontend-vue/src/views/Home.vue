<template>
  <article>
    <img alt="Vue logo" src="../assets/logo.png" />
    <header>
      <h1>{{ translationVm.header }}</h1>
    </header>
    <p>{{ translationVm.p1 }}</p>
    <WeatherForecast />
  </article>
</template>

<script setup>
import { ref } from "vue";
import WeatherForecast from "../components/weather-forecast.vue";
import { useTranslatorAsync } from "../plugins/translatorPlugin";
const translationVm = ref({});
useTranslatorAsync("views/Home", navigator.language).then(data => translationVm.value = data);
</script>

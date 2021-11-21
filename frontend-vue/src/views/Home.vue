<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png" />
    <WeatherForecast />
  </div>
</template>

<script>
// @ is an alias to /src
import WeatherForecast from "@/components/weather-forecast.vue";

export default {
  name: "Home",
  components: {
    WeatherForecast,
  },
};
</script>

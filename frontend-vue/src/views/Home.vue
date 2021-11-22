<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png" />
    <h1>'Hello world' leht</h1>
    <p>Kiire indikatsioon, et Web API käivitunud.</p>
    <WeatherForecast />
  </div>
</template>

<script>
// @ is an alias to /src
import WeatherForecast from "@/components/weather-forecast.vue";

export default {
  name: "Home",
  components: { WeatherForecast },
};
</script>

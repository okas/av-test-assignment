<template>
  <article>
    <img alt="Vue logo" src="../assets/logo.png" />
    <header>
      <h1>'Hello world' leht</h1>
    </header>
    <p>Kiire indikatsioon, et Web API käivitunud.</p>
    <WeatherForecast />
  </article>
</template>

<script>
// @ is an alias to /src
import WeatherForecast from "@/components/weather-forecast.vue";

export default {
  name: "Home",
  components: { WeatherForecast },
};
</script>

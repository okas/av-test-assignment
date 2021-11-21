<template>
  <div class="hello">
    <h1>'Hello world' leht</h1>
    <p>
      Kiire indikatsioon, et Web API käivitunud.
    </p>
    <h3>Ilmaennestuse info (API genereeritud)</h3>
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
        <tr v-for="(item, i) in data" :key="i">
          <td class="item" v-text="item.date"></td>
          <td class="item" v-text="item.temperatureC"></td>
          <td class="item" v-text="item.temperatureF"></td>
          <td class="item" v-text="item.summary"></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
  export default {
    name: "WeatherForecast",
    data() {
      return { data: [] }
    },
    beforeMount() {
      this.getForecast()
    },
    methods: {
      async getForecast() {
        const res = await fetch('/weatherforecast');
        const tempData = await res.json();
        // Push result array to data
        tempData.forEach(({ date, ...rest }) =>
          this.data.push(
            //Handle date presentation
            { date: new Date(date).toLocaleDateString('et'), ...rest }
          )
        );
      }
    }
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
</style>

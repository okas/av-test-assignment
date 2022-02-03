<template>
  <header class="main-header">
    <MainNavigation />
    <div class="global-loader-icon" v-if="$store.state.loading" />
  </header>
  <main>
    <router-view />
  </main>
  <aside></aside>
</template>

<script setup>
import MainNavigation from "./components/main-navigation.vue";
</script>

<style>
  html, body {
    margin: 0;
    min-height: 100%;
  }
  body {
    box-sizing: border-box;
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    background-image: linear-gradient(to top, #30cfd0 0%, #330867 100%);
  }

  #app-root {
    margin: 2rem;
    padding: 1.5rem;
    border-start-start-radius: 1.1rem 1.1rem;
    border-start-end-radius: 1.1rem 1.1rem;
    border-end-start-radius: 1.1rem 1.1rem;
    border-end-end-radius: 1.1rem 1.1rem;
    background-color: #eff5f5;
  }

  .main-header {
    position: relative;
    border-start-start-radius: 0.8rem 0.8rem;
    border-start-end-radius: 0.8rem 0.8rem;
    border-end-start-radius: 0.25rem 0.25rem;
    border-end-end-radius: 0.25rem 0.25rem;
    background: linear-gradient(to bottom, rgba(255,255,255,0.15) 0%, rgba(0,0,0,0.15) 100%), radial-gradient(at top center, rgba(255,255,255,0.40) 0%, rgba(0,0,0,0.40) 120%) #989898;
    background-blend-mode: multiply,multiply;
    /*background-image: linear-gradient(to top, lightgrey 0%, lightgrey 1%, #e0e0e0 26%, #efefef 48%, #d9d9d9 75%, #bcbcbc 100%);*/
  }

.global-loader-icon {
  position: absolute;
  left: 1.5rem;
  top: 1.5rem;
  border: 0.5rem solid #f3f3f3;
  border-top: 0.5rem solid #42b983;
  border-radius: 100%;
  width: 1rem;
  height: 1rem;
  animation: spin 0.5s linear infinite;
}

  @keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}
</style>

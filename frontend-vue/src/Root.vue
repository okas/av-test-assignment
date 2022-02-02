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
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.global-loader-icon {
  position: absolute;
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

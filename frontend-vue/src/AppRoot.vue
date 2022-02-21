<template>
  <header class="header-main">
    <TheGlobalLoaderIcon class="global-loader-icon-position" />
    <TheNavigationTop />
    <TheLanguageSelector class="language-selector-position" />
  </header>
  <main>
    <router-view />
  </main>
  <aside></aside>
</template>

<script setup>
import TheNavigationTop from "./components/the-navigation-top.vue";
import TheLanguageSelector from "./components/the-language-selector.vue";
import TheGlobalLoaderIcon from "./components/the-global-loader-icon.vue";
</script>

<style>
html,
body {
  margin: 0;
  min-height: 100%;
}

body {
  overflow-y: scroll;
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

.header-main {
  position: relative;
  border-start-start-radius: 0.8rem 0.8rem;
  border-start-end-radius: 0.8rem 0.8rem;
  border-end-start-radius: 0.25rem 0.25rem;
  border-end-end-radius: 0.25rem 0.25rem;
  background: linear-gradient(
      to bottom,
      rgb(255 255 255 / 15%) 0%,
      rgb(0 0 0 / 15%) 100%
    ),
    radial-gradient(
        at top center,
        rgb(255 255 255 / 40%) 0%,
        rgb(0 0 0 / 40%) 120%
      )
      #989898;
  background-blend-mode: multiply, multiply;

  /* background-image: linear-gradient(to top, lightgrey 0%, lightgrey 1%, #e0e0e0 26%, #efefef 48%, #d9d9d9 75%, #bcbcbc 100%); */
}

.global-loader-icon-position {
  position: absolute;
  left: 1.5rem;
  top: 1.5rem;
}

.language-selector-position {
  position: absolute;
  right: 1.5rem;
  top: 1.8rem;
}
</style>

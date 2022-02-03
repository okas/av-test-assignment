<template>
  <nav id="main-navigation" class="app-menu">
    <template v-for="{path, name} in $router.options.routes" :key="path">
      <router-link :to="path">{{ routeNamesTranslated[name] }}</router-link> |
    </template>
    <a class="external" href="https://localhost:5001/swagger" target="_blank">Swagger</a>
  </nav>
</template>

<script setup>
import { ref, watchEffect } from "vue";
import { useStore } from "vuex";
import { useTranslator } from "../plugins/translatorPlugin";

const routeNamesTranslated = ref({});
const store = useStore();

const translatorAsync = useTranslator();

watchEffect(async () => routeNamesTranslated.value = await translatorAsync("components/main-navigation", store.state.language));
</script>

<style scoped>
#main-navigation {
  padding: 30px;
}

#main-navigation a {
  font-weight: bold;
  color: #2c3e50;
}

#main-navigation a.router-link-exact-active {
  color: #42b983;
  filter: drop-shadow(0px 2px 6px #333300);
}

a.external[target="_blank"]::after {
content: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAQElEQVR42qXKwQkAIAxDUUdxtO6/RBQkQZvSi8I/pL4BoGw/XPkh4XigPmsUgh0626AjRsgxHTkUThsG2T/sIlzdTsp52kSS1wAAAABJRU5ErkJggg==);
margin: 0px 3px 0px 5px;
}
</style>

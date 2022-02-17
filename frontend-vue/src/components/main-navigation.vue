<template>
  <nav id="main-navigation" class="app-menu">
    <template v-for="{ path, name } in router.options.routes" :key="path">
      <router-link :to="path">{{ routeNamesTranslated[name] }}</router-link> |
    </template>
    <a class="external" href="https://localhost:5001/swagger" target="_blank"
      >Swagger</a
    >
  </nav>
</template>

<script setup>
import { ref, watchEffect } from "vue";
import { useRouter } from "vue-router";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const store = useRootStore();
const routeNamesTranslated = ref({
  Home: "",
  About: "",
  UserInteractions: "",
});

const router = useRouter();
const translatorAsync = useTranslator();

watchEffect(
  async () =>
    (routeNamesTranslated.value = await translatorAsync(
      "components/main-navigation",
      store.language
    ))
);
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
  filter: drop-shadow(0 2px 6px #330);
}

a.external[target="_blank"]::after {
  content: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAQElEQVR42qXKwQkAIAxDUUdxtO6/RBQkQZvSi8I/pL4BoGw/XPkh4XigPmsUgh0626AjRsgxHTkUThsG2T/sIlzdTsp52kSS1wAAAABJRU5ErkJggg==");
  margin: 0 3px 0 5px;
}
</style>

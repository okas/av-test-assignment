<script setup lang="ts">
import { ref, watchEffect } from "vue";
import { useRouter } from "vue-router";
import { TranslatorAsync, useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const store = useRootStore();
const routeNamesTranslated = ref(
  new Map([
    ["Home", ""],
    ["About", ""],
    ["UserInteractions", ""],
  ])
);

const router = useRouter();
const translatorAsync = useTranslator() as TranslatorAsync;

watchEffect(async () => {
  const data = await translatorAsync(
    "components/the-navigation-top",
    store.language
  );
  routeNamesTranslated.value = new Map(Object.entries(data));
});
</script>

<template>
  <nav id="navigation-top" class="app-menu">
    <template v-for="{ path, name } in router.options.routes" :key="path">
      <router-link
        :to="path"
        v-text="routeNamesTranslated.get(name?.toString() ?? '')"
      />
      |
    </template>
    <a
      class="swagger-link"
      href="https://localhost:5001/swagger"
      target="_blank"
    >
      Swagger
    </a>
  </nav>
</template>

<style scoped>
#navigation-top {
  padding: 1.75rem;
}

#navigation-top a {
  font-weight: bold;
  color: #2c3e50;
}

#navigation-top a.router-link-exact-active {
  color: #42b983;
  filter: drop-shadow(0 2px 6px #330);
}

.swagger-link::after {
  content: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAQElEQVR42qXKwQkAIAxDUUdxtO6/RBQkQZvSi8I/pL4BoGw/XPkh4XigPmsUgh0626AjRsgxHTkUThsG2T/sIlzdTsp52kSS1wAAAABJRU5ErkJggg==");
  margin: 0 3px 0 5px;
}
</style>

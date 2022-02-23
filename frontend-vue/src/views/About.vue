<script setup>
import { defineAsyncComponent, ref, watchEffect } from "vue";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const Tree = defineAsyncComponent(() => import("../components/tree.vue"));
const store = useRootStore();
const translationVm = ref(null);

const translatorAsync = useTranslator();

watchEffect(
  async () =>
    (translationVm.value = await translatorAsync("views/About", store.language))
);
</script>

<template>
  <article class="about">
    <Tree :data="translationVm" />
  </article>
</template>

<style scoped>
.about {
  text-align: justify;
}

.about section:not(:last-of-type) {
  margin-bottom: 2rem;
}
</style>

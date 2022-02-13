<template>
  <article class="about">
    <section>
      <header>
        <h1>{{ translationVm.section[0].header }}</h1>
      </header>
      <p>{{ translationVm.section[0].p1 }}</p>
      <ol>
        <li v-for="(item, i) in translationVm.section[0].list1" :key="i">
          {{ item }}
        </li>
      </ol>
      <p>{{ translationVm.section[0].p2 }}</p>
    </section>
    <section>
      <header>
        <h1>{{ translationVm.section[1].header }}</h1>
      </header>
      <p>{{ translationVm.section[1].p1 }}</p>
      <p>{{ translationVm.section[1].p2 }}</p>
      <ol>
        <li v-for="(item, i) in translationVm.section[1].list1" :key="i">
          {{ item }}
        </li>
      </ol>
    </section>
  </article>
</template>

<script setup>
import { ref, watchEffect } from "vue";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const store = useRootStore();

const translationVm = ref({
  section: [
    {
      header: "",
      p1: "",
      list1: [],
      p2: "",
    },
    {
      header: "",
      p1: "",
      p2: "",
      list1: [],
    },
  ],
});

const translatorAsync = useTranslator();

watchEffect(
  async () =>
    (translationVm.value = await translatorAsync(
      "views/about",
      store.language
    ))
);
</script>

<style scoped>
.about {
  text-align: justify;
}
.about section:not(:last-of-type) {
  margin-bottom: 2rem;
}
</style>

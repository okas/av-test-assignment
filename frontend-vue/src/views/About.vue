<template>
  <article class="about">
    <section>
      <header>
        <h1>{{ viewModel.section[0].header }}</h1>
      </header>
      <p>{{ viewModel.section[0].p1 }}</p>
      <ol>
        <li v-for="(item, i) in viewModel.section[0].list1" :key="i">{{ item }}</li>
      </ol>
      <p>{{ viewModel.section[0].p2 }}</p>
    </section>
    <section>
      <header>
        <h1>{{ viewModel.section[1].header }}</h1>
      </header>
      <p>{{ viewModel.section[1].p1 }}</p>
      <p>{{ viewModel.section[1].p2 }}</p>
      <ol>
        <li v-for="(item, i) in viewModel.section[1].list1" :key="i">{{ item }}</li>
      </ol>
    </section>
  </article>
</template>

<script setup>
import { ref } from "vue";
import { useTranslator } from "../plugins/translatorPlugin";

const viewModel = ref({
  section: [
    {
      header: "",
      p1: "",
      list1: [],
      p2: ""
    },
    {
      header: "",
      p1: "",
      p2: "",
      list1: []
    }
  ]
});

useTranslator()("views/about").then(data => viewModel.value = data);
</script>

<style scoped>
.about {
  text-align: justify;
}
.about section:not(:last-of-type) {
  margin-bottom: 2rem;
}
</style>

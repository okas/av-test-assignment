<template>
  <article class="about">
    <header>
      <h1>{{ viewModel.header }}</h1>
    </header>
    <p>{{ viewModel.p1 }}</p>
    <ol>
      <li v-for="(item, i) in viewModel.list1" :key="i">{{ item }}</li>
    </ol>
    <p>{{ viewModel.p2 }}</p>
  </article>
</template>

<script setup>
  import { ref } from "vue";
  import { useTranslatorAsync } from "../plugins/translatorPlugin";

  const viewModel = ref({
    header: "",
    p1: "",
    list1: [],
    p2: ""
  });

  useTranslatorAsync("views/about", navigator.language).then(data => viewModel.value = data);
</script>

<style scoped>
.about {
  text-align: justify;
}
</style>

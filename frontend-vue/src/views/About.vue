<template>
  <article class="about">
    <header>
      <h1>{{ viewModel.header }}</h1>
    </header>
    <p>{{ viewModel.p1 }}</p>
    <ol>
      <li v-for="(item, i) in viewModel.ol_items" :key="i">{{ item }}</li>
    </ol>
    <p>{{ viewModel.p2 }}</p>
  </article>
</template>

<script setup>
  import { computed } from "vue";
  import { useStore } from 'vuex'

  const store = useStore();

  const viewModel = computed(() => store.getters.getAboutComponentModel(navigator.language));
</script>

<style scoped>
.about {
  text-align: justify;
}
</style>

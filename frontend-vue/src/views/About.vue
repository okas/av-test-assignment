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
    <section>
      <header><h1>Testing</h1></header>
      <Tree :data="testTree" />
    </section>
  </article>
</template>

<script setup>
import { defineAsyncComponent, ref, watchEffect } from "vue";
import { useTranslator } from "../plugins/translatorPlugin";
import useRootStore from "../stores/app-store";

const Tree = defineAsyncComponent(() => import("../components/tree.vue"));

const testTree = [
  [
    {
      article: {
        section: {
          p: "list subject",
          span: ["span1", "span2"],
          div: {
            p: "section>div>p",
            span: ["section>div>p>span1", "section>div>p>span2"],
          },
          div_1: ["div1", "div2"],
        },
        section_1: [
          {
            div: {
              p: "section_1>div>p",
              p_1: ["section_1>div>p[0]", "section_1>div>p[1]"],
            },
          },
        ],
        ol: {
          li: ["ol>li[0]", "ol>li[1]"],
        },
      },
    },
  ],
];

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
    (translationVm.value = await translatorAsync("views/about", store.language))
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

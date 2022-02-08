<template>
  <div>
    <label for="global-language-selector">{{ selectedItem.label }}</label
    >&nbsp;
    <select
      id="global-language-selector"
      :value="selectedItem.iso"
      @change.stop="onLanguageChange"
    >
      <option disabled>{{ selectedItem.auxilliar }}</option>
      <option
        v-for="{ iso, native } in supportedLanguages"
        :key="iso"
        :value="iso"
      >
        {{ native }}
      </option>
    </select>
  </div>
</template>

<script setup>
import { computed, watchEffect } from "vue";
import { supportedLanguages } from "../translations/index";
import useRootStore from "../stores/app-store";

const store = useRootStore();

const selectedItem = computed(() =>
  supportedLanguages.find((item) => item.iso === store.language)
);

function onLanguageChange({ target: { value } }) {
  store.setLanguage(value);
}

watchEffect(() => (document.documentElement.lang = store.language));
</script>

<style scoped></style>

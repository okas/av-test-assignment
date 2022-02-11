<template>
  <div>
    <label for="global-language-selector">{{ selectedItem?.label }}</label
    >&nbsp;
    <select
      id="global-language-selector"
      :value="selectedItem?.iso"
      @change.stop="onLanguageChange"
    >
      <option disabled>{{ selectedItem?.auxilliar }}</option>
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
import { supportedLanguages, fallBackLanguage } from "../translations/index";
import useRootStore from "../stores/app-store";

const store = useRootStore();

const storage = window.localStorage;
const storageKey = "app:language";

let storedLanguage = storage.getItem(storageKey);

if (!storedLanguage) {
  // this should be first wisit ever to webapp
  // use browser language, if it is supportedlanguage; otherwise use fallback language
  storedLanguage = supportedLanguages.some(
    (lng) => lng.iso === navigator.language
  )
    ? navigator.language
    : fallBackLanguage;

  storage.setItem(storageKey, storedLanguage);
}

store.setLanguage(storedLanguage);

const selectedItem = computed(() =>
  supportedLanguages.find((item) => item.iso === store.language)
);

/** @param {Event & {target: EventTarget & {value: String}}} param0 */
function onLanguageChange({ target: { value } }) {
  store.setLanguage(value);
}

watchEffect(() => {
  document.documentElement.lang = store.language;
  storage.setItem(storageKey, store.language);
});
</script>

<style scoped></style>

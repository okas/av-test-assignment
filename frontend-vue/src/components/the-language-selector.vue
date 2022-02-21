<script setup>
import { computed } from "vue";
import { supportedLanguages } from "../translations/index";
import useRootStore from "../stores/app-store";

const store = useRootStore();

const selectedItem = computed(() =>
  supportedLanguages.find((item) => item.iso === store.language)
);

/** @param {Event & {target: EventTarget & {value: String}}} param0 */
function onLanguageChange({ target: { value } }) {
  store.setLanguage(value);
}
</script>

<template>
  <div>
    <label for="global-language-selector" v-text="selectedItem?.label" />
    &nbsp;
    <select
      id="global-language-selector"
      :value="selectedItem?.iso"
      @change.stop="onLanguageChange"
    >
      <option disabled v-text="selectedItem?.auxilliar" />
      <option
        v-for="{ iso, native } in supportedLanguages"
        :key="iso"
        :value="iso"
        v-text="native"
      />
    </select>
  </div>
</template>

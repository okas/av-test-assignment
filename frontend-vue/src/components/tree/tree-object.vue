<template>
  <template v-for="(value, key, i) in data" :key="i">
    <TreeArray
      v-if="Array.isArray(value)"
      :name="sanitizeTag(key)"
      :data="value"
    />

    <TreeString
      v-else-if="isString(value)"
      :name="sanitizeTag(key)"
      :data="value"
    />

    <component :is="sanitizeTag(key)" v-else-if="isObject(value)">
      <TreeObject :data="value" />
    </component>
  </template>
</template>

<script setup>
import { sanitizeTag } from "./common";
import { isObject, isString } from "../../utils/typeHelpers";
import TreeArray from "./tree-array.vue";
import TreeString from "./tree-string.vue";

defineProps({
  data: {
    type: Object,
    required: true,
  },
});
</script>

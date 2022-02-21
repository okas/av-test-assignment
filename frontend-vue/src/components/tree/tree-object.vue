<script setup>
import { sanitizeTag } from "./common";
import { isObject, isString } from "../../utils/typeHelpers";
import TreeArray from "./tree-array.vue";
import TreeString from "./tree-string.vue";

defineProps({
  name: {
    type: String,
    default: "",
  },
  data: {
    type: Object,
    required: true,
  },
});
</script>

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
      <TreeObject :name="sanitizeTag(key)" :data="value" />
    </component>
  </template>
</template>

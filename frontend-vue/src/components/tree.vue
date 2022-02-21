<script setup>
import { isObject } from "../utils/typeHelpers";

import TreeArray from "./tree/tree-array.vue";
import TreeObject from "./tree/tree-object.vue";

defineProps({
  data: {
    required: true,
    validator(value) {
      if (!(Array.isArray(value) || typeof value === "object")) {
        throw new Error(
          `Tree component 'data' property validation error: invalid type; must be either object or array, but was ${typeof value}.`
        );
      }
      return true;
    },
  },
});
</script>

<template>
  <TreeArray v-if="Array.isArray(data)" :data="data" />
  <TreeObject v-else-if="isObject(data)" :data="data" />
</template>

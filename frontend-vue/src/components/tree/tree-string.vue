<template>
  <component :is="sanitizeTag(name)">
    {{ data }}
  </component>
</template>

<script setup>
import { sanitizeTag } from "./common";

defineProps({
  name: {
    type: String,
    required: true,
  },
  data: {
    type: String,
    required: true,
  },
});
</script>

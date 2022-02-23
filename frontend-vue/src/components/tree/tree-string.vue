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

<template>
  <component :is="sanitizeTag(name)" v-text="data" />
</template>

<script setup>
import { sanitizeTag } from "./common";
import { isObject, isString } from "../../utils/typeHelpers";
import TreeObject from "./tree-object.vue";
import TreeString from "./tree-string.vue";

defineProps({
  name: {
    type: String,
    default: "",
  },
  data: {
    type: Array,
    required: true,
  },
});
</script>

<template>
  <template v-for="(item, i) in data" :key="i">
    <TreeArray
      v-if="Array.isArray(item)"
      :name="sanitizeTag(name)"
      :data="item"
    />
    <TreeString
      v-else-if="isString(item)"
      :name="sanitizeTag(name)"
      :data="item"
    />
    <template v-else-if="isObject(item)">
      <component :is="sanitizeTag(name)" v-if="name">
        <TreeObject :name="sanitizeTag(name)" :data="item" />
      </component>
      <TreeObject v-else :name="sanitizeTag(name)" :data="item" />
    </template>
  </template>
</template>

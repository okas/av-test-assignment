<template>
  <input type="datetime-local" v-model="dataValueClientFormatted" />
</template>

<script setup>
  import { defineProps, defineEmits, computed } from "vue";
  import formatDateMixin from "../mixins/formatDateMixin";

  const props = defineProps({
    datevalue: Date // TODO Throw, because by default string will only warn, but programm will continue execution.
  });

  const emit = defineEmits({
    "update:datevalue": (value) => new Date(value) !== "Invalid Date" // TODO does it flow through like the prop validation?
  })

  const dataValueClientFormatted = computed({
    get: () => formatDateMixin.methods.formatToHtmlStringDateTime(props.datevalue),
    set: (value) => emit("update:datevalue", new Date(value))
  });
</script>

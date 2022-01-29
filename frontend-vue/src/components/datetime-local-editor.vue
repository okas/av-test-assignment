<template>
  <input type="datetime-local" v-model="dataValueClientFormatted" />
</template>

<script setup>
import { defineProps, defineEmits, computed } from "vue";
import useFormatDateTime from "../utils/formatDateTime";

const props = defineProps({
  datevalue: {
    type: Date,
    required: true,
    validator(value) {
      if (typeof (new Date(value)) !== "object") {
        throw new Error(`"datevalue" property validation error: value is not valid date! Attempted value: ${value}`);
      }
      return true;
    }
  },
});

const emit = defineEmits({
  "update:datevalue": (value) => typeof (new Date(value)) === "object",
})

const dataValueClientFormatted = computed({
  get: () => formatToHtmlStringDateTime(props.datevalue),
  set: value => emit("update:datevalue", new Date(value))
});

const { formatToHtmlStringDateTime } = useFormatDateTime();
</script>

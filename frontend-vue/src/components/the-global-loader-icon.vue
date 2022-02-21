<script setup>
import useRootStore from "../stores/app-store";
const store = useRootStore();
</script>

<template>
  <div v-if="store.loading" class="global-loader-icon" />
</template>

<style scoped>
.global-loader-icon {
  border: 0.5rem solid #f3f3f3;
  border-top: 0.5rem solid #42b983;
  border-radius: 100%;
  width: 1rem;
  height: 1rem;
  animation: spin 0.5s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}
</style>

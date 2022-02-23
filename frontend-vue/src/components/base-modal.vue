<script setup lang="ts">
import { watchEffect } from "vue";
import { UseFocusTrap } from "@vueuse/integrations/useFocusTrap/component";
import ButtonClose from "./button-close.vue";

const props = defineProps({
  show: { required: true, type: Boolean },
});

const emit = defineEmits(["close"]);

watchEffect(() => {
  if (props.show) {
    document.querySelector("body")?.classList.add("overflow-hidden");
  } else {
    document.querySelector("body")?.classList.remove("overflow-hidden");
  }
});

function closeModal() {
  emit("close");
}
</script>
<!-- TODO: focus config to first input element? -->
<template>
  <teleport to="body">
    <transition name="fade">
      <UseFocusTrap v-if="show" class="modal" as="div" @keyup.esc="closeModal">
        <div class="modal--backdrop" @click.self="closeModal" />
        <div class="modal--dialog">
          <header class="modal--header">
            <slot name="header" />
            <ButtonClose @click="closeModal" />
          </header>
          <section class="modal--body">
            <slot name="default" />
          </section>
          <footer class="modal--footer">
            <slot name="footer" />
          </footer>
        </div>
      </UseFocusTrap>
    </transition>
  </teleport>
</template>

<style>
.overflow-hidden {
  overflow: hidden;
}
</style>

<style lang="scss" scoped>
.modal {
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 9;

  &--backdrop {
    background-color: rgb(41 102 78 / 36%);
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    z-index: 1;
  }

  &--dialog {
    background-color: #fff;
    position: relative;
    top: 9.2rem;
    width: 600px;
    margin: 50px auto;
    display: flex;
    flex-direction: column;
    border-radius: 5px;
    z-index: 2;
    @media screen and (max-width: 992px) {
      width: 90%;
    }
  }

  &--header {
    padding: 20px 20px 10px;
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
  }

  &--body {
    padding: 10px 20px;
    overflow: auto;
    display: flex;
    flex-direction: column;
    align-items: stretch;
  }

  &--footer {
    padding: 10px 20px 20px;
  }
}

.fade-enter-active {
  transition: opacity 0.15s;
}

.fade-leave-active {
  transition: opacity 0.25s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

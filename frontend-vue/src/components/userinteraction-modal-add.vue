<script setup lang="ts">
import { ref, watchEffect } from "vue";
import { TranslatorAsync, useTranslator } from "../plugins/translatorPlugin";
import { useConfirmDialog } from "@vueuse/core";
import useRootStore from "../stores/app-store";
import ModalBase from "./base-modal.vue";
import IconSave from "../assets/svg-material/save_black_24dp.svg?component";
import DateTimeLocalEditor from "./input-datetime-local.vue";
import { useRoundToNext15Minutes } from "../utils/dateTimeHelpers";
import { IInteractionAdd } from "../models/Interaction";

class ViewModel implements IInteractionAdd {
  description: string;
  deadline: Date;
  constructor(description = "", deadline = useRoundToNext15Minutes()) {
    this.description = description;
    this.deadline = deadline;
  }
}

const store = useRootStore();

const translatedVm = ref({
  title: "",
  form: {
    description: "",
    deadline: "",
  },
});

const viewModel = ref<ViewModel | null>(null);

const dialog = useConfirmDialog<IInteractionAdd, IInteractionAdd, null>();

dialog.onReveal(() => (viewModel.value = new ViewModel()));

dialog.onCancel(() => (viewModel.value = null));

dialog.onConfirm(() => (viewModel.value = null));

const translatorAsync = useTranslator() as TranslatorAsync;

defineExpose({ dialog });

watchEffect(async () => {
  translatedVm.value = await translatorAsync(
    "components/userinteraction-modal-add",
    store.language
  );
});
</script>
<!-- TODO: focus config to first input element? -->
<template>
  <ModalBase :show="dialog.isRevealed.value" @close="dialog.cancel">
    <template #header>
      <h1 v-text="translatedVm.title" />
    </template>
    <form @submit.prevent="dialog.confirm(viewModel!)">
      <fieldset class="form-grid">
        <label
          class="label"
          for="new-description"
          v-text="translatedVm.form.description"
        />
        <input
          id="new-description"
          v-model="viewModel!.description"
          class="control"
          type="text"
          :placeholder="translatedVm.form.description"
          size="50"
        />
        <label
          class="label"
          for="new-deadline"
          v-text="translatedVm.form.deadline"
        />
        <DateTimeLocalEditor
          id="new-deadline"
          v-model:datevalue="viewModel!.deadline"
          class="control"
        />
      </fieldset>
    </form>
    <template #footer>
      <button @click="dialog.confirm(viewModel!)">
        <IconSave />
      </button>
    </template>
  </ModalBase>
</template>

<style scoped lang="scss">
.form-grid {
  border: 0;
  padding: 0;
  margin: 0;
  display: grid;
  grid-template-columns: auto auto;
  grid-template-rows: auto auto;
  gap: 1.5rem 1rem;

  & .label {
    justify-self: end;
  }

  & .control {
    font-size: inherit;
    font-family: inherit;
    border: 0;
    box-shadow: 1px 1px 5px 4px rgb(18 184 65 / 15%);
    border-radius: 0.25rem;
  }
}
</style>

<script setup lang="ts">
import { ref } from "vue";
import { useConfirmDialog } from "@vueuse/core";
import ModalBase from "./base-modal.vue";
import DateTimeLocalEditor from "./input-datetime-local.vue";
import { useRoundToNext15Minutes } from "../utils/dateTimeHelpers";

class ViewModel {
  description: string;
  deadline: Date;

  constructor(description = "", deadline = useRoundToNext15Minutes()) {
    this.description = description;
    this.deadline = deadline;
  }
}

const translatedVm = ref({
  title: "Add new interaction",
  section_form: {
    description_placeholder: "description",
    deadline_label: "deadline",
    submit_text: "add new",
  },
});

const viewModel = ref<ViewModel | null>(null);

const dialog = useConfirmDialog();

dialog.onReveal(() => (viewModel.value = new ViewModel()));

dialog.onCancel(() => (viewModel.value = null));

dialog.onConfirm(() => (viewModel.value = null));

defineExpose({ dialog });
</script>
<!-- TODO: focus config to first input element? -->
<template>
  <ModalBase :show="dialog.isRevealed.value" @close="dialog.cancel">
    <template #header>
      <h1 v-text="translatedVm.title" />
    </template>
    <form @submit.prevent="dialog.confirm(viewModel)">
      <fieldset class="form-grid">
        <label
          class="label"
          for="new-description"
          v-text="translatedVm.section_form.description_placeholder"
        />
        <input
          id="new-description"
          v-model="viewModel!.description"
          class="control"
          type="text"
          :placeholder="translatedVm.section_form.description_placeholder"
          size="50"
        />
        <label
          class="label"
          for="new-deadline"
          v-text="translatedVm.section_form.deadline_label"
        />
        <DateTimeLocalEditor
          id="new-deadline"
          v-model:datevalue="viewModel!.deadline"
          class="control"
        />
      </fieldset>
    </form>
    <template #footer>
      <button @click="dialog.cancel">Cancel</button>
      <button
        @click="dialog.confirm(viewModel)"
        v-text="translatedVm.section_form.submit_text"
      />
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

<script setup lang="ts">
import { computed, ref, watch, watchEffect } from "vue";
import { TranslatorAsync, useTranslator } from "../plugins/translatorPlugin";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import useFormatDateTime from "../utils/formatDateTime";
import { useRemoveOne } from "../utils/arrayHelpers";
import useRootStore from "../stores/app-store";
import InputDateLocal from "../components/input-datetime-local.vue";
import ModalAdd from "../components/userinteraction-modal-add.vue";
import {
  IInteractionVm,
  IInteractionAdd,
  IInteractionEdit,
} from "../models/Interaction";
// TODO: test dynamic component, especially its lifetime consternes and :ref template prop co-op.

// const ModalAdd = defineAsyncComponent(() =>
//   import("../components/userinteraction-modal-add.vue")
// );

class ViewModel implements IInteractionVm {
  id: string;
  description: string;
  deadline: Date;
  created: Date;
  isOpen: boolean;
  constructor({ id, description, deadline, created, isOpen }: any) {
    this.id = id;
    this.description = description;
    this.deadline = new Date(deadline);
    this.created = new Date(created);
    this.isOpen = isOpen;
  }
}

class EditViewModel implements IInteractionEdit {
  id: string;
  description: string;
  deadline: Date;
  isOpen: boolean;
  constructor({ id, description, deadline, isOpen }: IInteractionVm) {
    this.id = id;
    this.description = description;
    this.deadline = new Date(deadline);
    this.isOpen = isOpen;
  }
}

const store = useRootStore();

const translatedVm = ref({
  header: "",
  section_list: {
    header: "",
    table_header: [],
  },
  confirmMessage: "",
});

const interactions = ref<IInteractionVm[]>([]);

const modalAdd = ref<InstanceType<typeof ModalAdd> | null>(null);

const api = useApiClient();

const { formatDateTimeShortDateShortTime: formatDateTimeToShort } =
  useFormatDateTime();

const interactionToEdit = ref<IInteractionEdit | null>(null);

const savingToApi = ref(false);

const translatorAsync = useTranslator() as TranslatorAsync;

getInteractions();

function getInteractions() {
  api
    .then((client) =>
      client.execute({
        operationId: "get_api_userinteractions",
        parameters: { isOpen: true },
      })
    )
    .then((resp) => {
      if (resp.ok)
        interactions.value = resp.body.map((raw: any) => new ViewModel(raw));
    });
}

function markInteractionClosed(id: string) {
  if (!confirm(translatedVm.value.confirmMessage)) {
    return;
  }
  api
    .then((client) =>
      client.execute({
        operationId: "patch_api_userinteractions__id_",
        parameters: { id },
        requestBody: { id, isOpen: false },
      })
    )
    .then((resp) => {
      if (resp.ok) {
        useRemoveOne(interactions.value, (item) => item.id === id);
      }
    });
}

function hasExpirationProblem(deadline: Date) {
  let toCompare = new Date();
  toCompare.setHours(toCompare.getHours() + 1);
  return deadline < toCompare;
}

async function openAddDialog(): Promise<void> {
  if (!modalAdd?.value) {
    return;
  }
  const { data, isCanceled } = await modalAdd.value.dialog.reveal();
  if (!isCanceled) {
    addNewInteraction(data as IInteractionAdd);
  }
}

function addNewInteraction({ description, deadline }: IInteractionAdd) {
  api
    .then((client) =>
      client.execute({
        operationId: "post_api_userinteractions",
        requestBody: { description, deadline: new Date(deadline) },
      })
    )
    .then((resp) => {
      if (resp.ok) {
        interactions.value.push(new ViewModel(resp.body));
      }
    });
}

async function editInteraction() {
  return await api
    .then((client) =>
      client.execute({
        operationId: "put_api_userinteractions",
        requestBody: interactionToEdit.value,
      })
    )
    .then((resp) => {
      if (resp.ok) {
        const id = interactionToEdit.value?.id;
        Object.assign(
          interactions.value.find((item) => item.id === id),
          interactionToEdit.value
        );
      }
    });
}

function openEditor(id: string) {
  const toEdit = interactions.value.find((item) => item.id === id);
  if (!toEdit) {
    throw new Error(
      `Cannot start editing Interaction {id: ${id}}, it is not found!`
    );
  }
  // New object to eliminate reference.
  interactionToEdit.value = new EditViewModel(toEdit);
}

function closeEditor() {
  interactionToEdit.value = null;
}

async function saveEditChanges() {
  savingToApi.value = true;
  console.log("saving...");
  await editInteraction();
  savingToApi.value = false;
  closeEditor();
}

function isEditorShown(id: string) {
  return interactionToEdit?.value?.id === id;
}

watchEffect(async () => {
  translatedVm.value = await translatorAsync(
    "views/UserInteractions",
    store.language
  );
});

/** Keep table sorted by deadline desc. always.*/
watch(
  computed(() => interactions.value.map((i) => i.deadline)),
  () => interactions.value.sort((a, b) => +a.deadline - +b.deadline),
  { deep: true }
);
</script>

<template>
  <ModalAdd ref="modalAdd" />
  <article class="user-interaction">
    <header>
      <h1 v-text="translatedVm.header" />
    </header>
    <fieldset class="tools" :disabled="modalAdd?.dialog.isRevealed.value">
      <button class="icon-add" @click="openAddDialog">+</button>
      <button class="icon-refresh" @click="getInteractions">↻</button>
    </fieldset>
    <section>
      <header>
        <h4 v-text="translatedVm.section_list.header" />
      </header>
      <table class="interaction-list">
        <thead>
          <tr>
            <th
              v-for="(headerName, i) in translatedVm.section_list.table_header"
              :key="i"
              :colspan="i > 0 ? undefined : 2"
              v-text="headerName"
            />
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="{ id, description, deadline, created } in interactions"
            :key="id"
            class="item"
            :class="{
              'item-dueproblem': hasExpirationProblem(deadline),
              'item-editor': isEditorShown(id),
            }"
            @keyup.enter="isEditorShown(id) ? saveEditChanges : undefined"
            @dblclick.stop="openEditor(id)"
          >
            <td class="item-openstate">
              <input
                v-if="isEditorShown(id)"
                v-model="interactionToEdit!.isOpen"
                type="checkbox"
              />
              <button v-else @click="markInteractionClosed(id)">✔</button>
            </td>
            <td class="item-description">
              <input
                v-if="isEditorShown(id)"
                v-model="interactionToEdit!.description"
                required
              />
              <template v-else>
                {{ description }}
              </template>
            </td>
            <td class="item-deadline">
              <InputDateLocal
                v-if="isEditorShown(id)"
                v-model:datevalue="interactionToEdit!.deadline"
                required
              />
              <template v-else>
                {{ formatDateTimeToShort(deadline, store.language) }}
              </template>
            </td>
            <td
              :class="isEditorShown(id) ? 'item-actions-aux' : 'item-created'"
              :colspan="isEditorShown(id) ? 2 : undefined"
            >
              <fieldset
                v-if="isEditorShown(id)"
                class="aux-controls"
                :disabled="savingToApi"
              >
                <button alt="dsefasec" @click="saveEditChanges" />
                <button @click="closeEditor" />
              </fieldset>
              <template v-else>
                {{ formatDateTimeToShort(created, store.language) }}
              </template>
            </td>
            <td v-if="!isEditorShown(id)" class="item-action">
              <button :disabled="savingToApi" @click="openEditor(id)">↪</button>
            </td>
          </tr>
        </tbody>
      </table>
    </section>
  </article>
</template>

<style scoped lang="scss">
@use "sass:color";

$outline-len: 0.12rem;

fieldset {
  border: none;
}

th,
td {
  padding: 0.75rem 0.5rem;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}

thead tr {
  border-bottom: $outline-len #42b983 solid;
}

.interaction-list {
  table-layout: auto;
  border-collapse: separate;
  border-spacing: 0 $outline-len * 2;
  border: $outline-len #42b983 solid;
  width: 100%;
}

.item-openstate {
  text-align: center;
}

.item-description {
  text-align: justify;
  width: 100%;

  input {
    width: 100%;
  }
}

.item-deadline,
.item-created {
  text-align: right;
}

.aux-controls {
  margin: 0;
  padding: 0;
  display: flex;
  justify-content: space-around;
}

.item-action {
  text-align: center;
}

.icon {
  &-refresh,
  &-add {
    height: 2rem;
    width: 2rem;
    font-size: 1rem;
    line-height: 1rem;
    font-weight: 900;
  }

  &-refresh {
    color: #14b8b8;
  }

  &-add {
    color: #8a14b8;
  }
}

tr.item {
  $border-radius-len: 0.25rem;
  $normal-bg-color: rgb(0 255 34 / 10%);

  &:nth-of-type(odd) {
    background-color: color.scale($normal-bg-color, $alpha: -50%);
  }

  &:nth-of-type(even) {
    background-color: $normal-bg-color;
  }

  &:hover {
    border-radius: $border-radius-len;
    outline: $outline-len ridge rgb(148 50 205);
  }

  td:first-of-type {
    border-radius: $border-radius-len 0 0 $border-radius-len;
  }

  td:last-of-type {
    border-radius: 0 $border-radius-len $border-radius-len 0;
  }

  &-editor {
    font-weight: 700;
    background-color: #faffe6;
    border-radius: $border-radius-len;
    outline: $outline-len solid rgb(0 195 255) !important;

    & .item-openstate input {
      margin: 0;
      padding: 0;
      transform: scale(1.5);
    }
  }

  &-dueproblem {
    $problem-bg-color: rgb(255 230 230 / 75%);

    &:nth-of-type(odd) {
      background-color: $problem-bg-color;
    }

    &:nth-of-type(even) {
      background-color: color.scale($problem-bg-color, $alpha: -40%);
    }
  }
}
</style>

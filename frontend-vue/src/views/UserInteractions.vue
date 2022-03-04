<script setup lang="ts">
import { computed, ref, watch, watchEffect } from "vue";
import { TranslatorAsync, useTranslator } from "../plugins/translatorPlugin";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import useFormatDateTime from "../utils/formatDateTime";
import useRootStore from "../stores/app-store";
import ModalAdd from "../components/userinteraction-modal-add.vue";
import { IInteractionAdd, IInteractionVm } from "../models/Interaction";
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

const store = useRootStore();

const translatedVm = ref({
  header: "",
  section_list: {
    header: "",
    table_header: [],
  },
  section_form: { submit_text: "add new" },
  confirmMessage: "",
});

const interactions = ref<IInteractionVm[]>([]);

const modalAdd = ref<InstanceType<typeof ModalAdd> | null>(null);

const api = useApiClient();

const { formatDateTimeShortDateShortTime: formatDateTimeToShort } =
  useFormatDateTime();

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

function markInteractionClosed(interaction) {
  if (!confirm(translatedVm.value.confirmMessage)) {
    return;
  }
  api
    .then((client) =>
      client.execute({
        operationId: "patch_api_userinteractions__id_",
        parameters: { id: interaction.id },
        requestBody: { id: interaction.id, isOpen: false },
      })
    )
    .then((resp) => {
      if (resp.ok)
        interactions.value.splice(interactions.value.indexOf(interaction), 1);
    });
}

function isProblematic(interaction: IInteractionVm) {
  let toCompare = new Date();
  toCompare.setHours(toCompare.getHours() + 1);
  return interaction.deadline < toCompare;
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
    <section>
      <div class="">
        <button
          class="icon-add"
          :disabled="modalAdd?.value?.dialog.isRevealed.value"
          @click="openAddDialog"
          v-text="translatedVm.section_form.submit_text"
        ></button>
      </div>
      <div class="">
        <button class="icon-refresh" @click="getInteractions">↻</button>
      </div>
    </section>
    <section>
      <header>
        <h4 v-text="translatedVm.section_list.header" />
      </header>
      <table>
        <thead>
          <tr>
            <th
              v-for="(headerName, i) in translatedVm.section_list.table_header"
              :key="i"
              v-text="headerName"
            />
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in interactions"
            :key="item.id"
            :class="{ 'due-problem': isProblematic(item) }"
          >
            <td class="item-description" v-text="item.description" />
            <td v-text="formatDateTimeToShort(item.deadline, store.language)" />
            <td v-text="formatDateTimeToShort(item.created, store.language)" />
            <td class="item-action">
              <button @click="markInteractionClosed(item)">✔</button>
              <button>↪</button>
            </td>
          </tr>
        </tbody>
      </table>
    </section>
  </article>
</template>

<style scoped lang="scss">
table {
  table-layout: auto;
  border-spacing: 0;
  border: 1px #42b983 solid;
  width: 100%;
}

td,
th {
  padding: 0.75rem 0.5rem;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}

td {
  text-align: right;
}

th {
  border-bottom: 1px #42b983 solid;
}

tbody tr:nth-child(even) {
  background-color: #f2f2f2;
}

tbody tr:hover {
  background: #daf1f1;
  outline: 1px solid black;
}

.item-description {
  text-align: justify;
}

.item-action {
  text-align: center;
}

tbody tr.due-problem {
  outline: 1px solid red;
  background-color: #ffe6e6;
}

tbody tr.due-problem:hover {
  outline: 1px solid black;
}

@mixin icon {
  height: 2rem;
  font-size: 1rem;
  line-height: 1rem;
}

.icon {
  &-refresh {
    @include icon;

    width: 2rem;
    color: #14b8b8;
    font-weight: 900;
  }

  &-add {
    @include icon;
  }
}
</style>

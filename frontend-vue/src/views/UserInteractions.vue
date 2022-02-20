<template>
  <article class="user-interaction">
    <header>
      <h1>{{ translatedVm.header }}</h1>
    </header>
    <section>
      <modal ref="modalName">
        <template #header>
          <h1>Modal title</h1>
        </template>

        <template #body>
          <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
            eiusmod tempor incididunt ut labore et dolore magna aliqua. Nunc sed
            velit dignissim sodales ut eu sem integer vitae. Id aliquet lectus
            proin nibh nisl condimentum. Fringilla urna porttitor rhoncus dolor
            purus. Nam aliquam sem et tortor. Nisl vel pretium lectus quam id.
            Cras pulvinar mattis nunc sed. Quis ipsum suspendisse ultrices
            gravida dictum fusce ut placerat orci. Tristique magna sit amet
            purus. Fermentum et sollicitudin ac orci phasellus egestas tellus.
            Erat pellentesque adipiscing commodo elit at imperdiet dui accumsan.
            Felis eget nunc lobortis mattis aliquam faucibus. Tincidunt eget
            nullam non nisi est sit amet facilisis. Mi in nulla posuere
            sollicitudin aliquam ultrices sagittis orci. Vitae proin sagittis
            nisl rhoncus mattis rhoncus urna neque. Eget nunc scelerisque
            viverra mauris in aliquam sem fringilla ut. Nec nam aliquam sem et
            tortor consequat id. Commodo nulla facilisi nullam vehicula ipsum a.
            Elementum tempus egestas sed sed. Faucibus purus in massa tempor nec
            feugiat nisl pretium fusce.
          </p>
          <p>
            Arcu cursus vitae congue mauris rhoncus aenean. Tempor id eu nisl
            nunc mi. Pharetra diam sit amet nisl suscipit adipiscing bibendum.
            Ut faucibus pulvinar elementum integer enim. Odio facilisis mauris
            sit amet massa vitae tortor condimentum lacinia. Eu non diam
            phasellus vestibulum lorem sed risus. Porttitor rhoncus dolor purus
            non enim praesent. Sit amet mauris commodo quis imperdiet. Lobortis
            feugiat vivamus at augue eget. Nibh tellus molestie nunc non
            blandit. Tellus mauris a diam maecenas sed enim ut. Tortor aliquam
            nulla facilisi cras fermentum odio eu feugiat pretium. Mattis
            aliquam faucibus purus in massa.
          </p>
        </template>

        <template #footer>
          <div>
            <button @click="$refs.modalName.closeModal()">Cancel</button>
            <button @click="$refs.modalName.closeModal()">Save</button>
          </div>
        </template>
      </modal>

      <form class="form" @submit.prevent>
        <div class="control">
          <button @click="$refs.modalName.openModal">℗</button>
        </div>
        <div class="control">
          <input
            id="new-description"
            v-model="newInteraction.description"
            type="text"
            :placeholder="translatedVm.section_form.description_placeholder"
            size="50"
          />
        </div>
        <div class="control">
          <input
            id="new-deadline"
            v-model="newInteraction.deadline"
            type="datetime-local"
          />
          <label for="new-deadline">
            : {{ translatedVm.section_form.deadline_label }}</label
          >
        </div>
        <div class="control">
          <button @click="addNewInteraction(newInteraction)">
            {{ translatedVm.section_form.submit_text }}
          </button>
        </div>
        <div class="control">
          <button class="refresh-icon" @click="getInteractions">↻</button>
        </div>
      </form>
    </section>
    <section>
      <header>
        <h4>{{ translatedVm.section_list.header }}</h4>
      </header>
      <table>
        <thead>
          <tr>
            <th
              v-for="(item, i) in translatedVm.section_list.table_header"
              :key="i"
            >
              {{ item }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in interactions"
            :key="item.id"
            :class="{ 'due-problem': isProblematic(item) }"
          >
            <td class="item-description">
              <span v-text="item.description" />&nbsp;
              <input
                v-model="item.description"
                type="text"
                :placeholder="item.description"
                style="border: none; font-size: inherit"
              />
            </td>
            <td>
              <span
                v-text="
                  formatDateTimeShortDateShortTime(
                    item.deadline,
                    store.language
                  )
                "
              />
              <DateTimeLocalEditor
                v-model:datevalue="item.deadline"
                style="border: none; font-size: inherit"
              />
            </td>
            <td>
              <span
                v-text="
                  formatDateTimeShortDateShortTime(item.created, store.language)
                "
              />
            </td>
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

<script setup>
import { reactive, ref, watch, watchEffect } from "vue";
import DateTimeLocalEditor from "../components/datetime-local-editor.vue";
import { useTranslator } from "../plugins/translatorPlugin";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import useFormatDateTime from "../utils/formatDateTime";
import useRootStore from "../stores/app-store";
import Modal from "../components/modal.vue";

const store = useRootStore();
const translatedVm = ref({
  header: "",
  section_form: {
    description_placeholder: "",
    deadline_label: "",
    submit_text: "",
  },
  section_list: {
    header: "",
    table_header: [],
  },
  confirmMessage: "",
});
const interactions = ref([
  {
    id: "",
    description: "",
    created: 0,
    deadline: 0,
  },
]);
const newInteraction = reactive({ description: "", deadline: "" });

const api = useApiClient();
const { formatDateTimeShortDateShortTime } = useFormatDateTime();
const translatorAsync = useTranslator();

watchEffect(
  async () =>
    (translatedVm.value = await translatorAsync(
      "views/UserInteractions",
      store.language
    ))
);

/** Keep table sorted by deadline desc. always.*/
watch(
  // TODO to watchPostEffect?
  interactions, // TODO Is lodash.cloneDeep required to watch deeply nested props, in arrays?
  () => interactions.value.sort((a, b) => a.deadline - b.deadline),
  { immediate: true, deep: true }
);

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
      if (resp.ok) interactions.value = resp.body.map(convertToVM);
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

function isProblematic(interaction) {
  let toCompare = new Date();
  toCompare.setHours(toCompare.getHours() + 1);
  return interaction.deadline < toCompare;
}

/** Convert dates to JS Date instances from response. */
function convertToVM({ created, deadline, ...rest }) {
  return {
    created: new Date(created),
    deadline: new Date(deadline),
    ...rest,
  };
}

function addNewInteraction({ description, deadline }) {
  api
    .then((client) =>
      client.execute({
        operationId: "post_api_userinteractions",
        requestBody: { description, deadline: new Date(deadline) },
      })
    )
    .then((resp) => {
      if (resp.ok) {
        interactions.value.push(convertToVM(resp.body));
        newInteraction.description = "";
        newInteraction.deadline = "";
      }
    });
}
</script>

<style>
.overflow-hidden {
  overflow: hidden;
}
</style>

<style scoped>
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

.form {
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  gap: 1rem;
}

.control {
  align-self: auto;
  flex-grow: 4;
}

.refresh-icon {
  font-weight: 700;
  font-size: 1rem;
  line-height: 1rem;
  color: #14b8b8;
}
</style>

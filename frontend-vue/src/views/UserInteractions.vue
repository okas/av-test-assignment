<template>
  <article class="user-interaction">
    <header>
      <h1>{{ translationVm.header }}</h1>
    </header>
    <section>
      <div class="form-container">
        <div class="control">
          <input type="text"
                 id="new-description"
                 :placeholder="translationVm.section_form.description_placeholder"
                 size="50"
                 v-model="newInteraction.description"
                 />
        </div>
        <div class="control">
          <input type="datetime-local"
                 id="new-deadline"
                 v-model="newInteraction.deadline" 
                 />
          <label for="new-deadline"> : {{ translationVm.section_form.deadline_label }}</label>
        </div>
        <div class="control">
          <button @click="addNewInteraction(newInteraction)"
                  >{{ translationVm.section_form.submit_text }}</button>
        </div>
        <div class="control">
          <button class="refresh-icon" @click="getInteractions">↻</button>
        </div>
      </div>
    </section>
    <section>
      <header>
        <h4>{{ translationVm.section_list.header }}</h4>
      </header>
      <table>
        <thead>
          <tr>
            <th v-for="(item, i) in translationVm.section_list.table_header" :key="i">{{ item }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in interactions" :key="item.id" :class="{ 'due-problem': isProblematic(item) }">
            <td class="item-description">
              <span v-text="item.description" />&nbsp;
              <input type="text" :placeholder="item.description" v-model="item.description"
                     style="border: none; font-size: inherit;" />
            </td>
            <td>
              <span v-text="formatDateTimeShortDateShortTime(item.deadline)" />
              <DateTimeLocalEditor v-model:datevalue="item.deadline"
                                   style="border: none; font-size: inherit; " />
            </td>
            <td>
              <span  v-text="formatDateTimeShortDateShortTime(item.created)"/>
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
import { reactive, ref, watch, onBeforeMount } from "vue";
import DateTimeLocalEditor from "../components/datetime-local-editor.vue";
import { useTranslator } from "../plugins/translatorPlugin";
import { useApiClient } from "../plugins/swaggerClientPlugin";
import useFormatDateTime from "../utils/formatDateTime";

const interactions = ref([]);
const translationVm = ref({
      header: "",
      section_form: {
        description_placeholder: "" ,
        deadline_label: "",
        submit_text: ""
      },
      section_list: {
        header: "",
        table_header: [],
      },
    });
const newInteraction = reactive({ description: "", deadline: "" });
const api = useApiClient();

useTranslator()("views/UserInteractions").then(data => translationVm.value = data);

onBeforeMount(getInteractions);

const { formatDateTimeShortDateShortTime } = useFormatDateTime();

async function getInteractions() {
  const resp = await api.then((client) =>
    client.execute({
      operationId: "get_api_userinteractions",
      parameters: { isOpen: true },
    })
  );
  if (resp.ok) {
    interactions.value = resp.body.map(convertToVM);
  }
}

async function markInteractionClosed(interaction) {
  if (!confirm("Kinnita pöördumise sulgemine")) {
    return;
  }
  const resp = await api.then((client) =>
    client.execute({
      operationId: "patch_api_userinteractions__id_",
      parameters: { id: interaction.id },
      requestBody: { id: interaction.id, isOpen: false },
    })
  );
  if (resp.ok) {
    interactions.value.splice(interactions.value.indexOf(interaction), 1);
  }
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

async function addNewInteraction({ description, deadline }) {
  const resp = await api.then((client) =>
    client.execute({
      operationId: "post_api_userinteractions",
      requestBody: { description, deadline: new Date(deadline) },
    })
  );
  if (resp.ok) {
    interactions.value.push(convertToVM(resp.body));
    newInteraction.description = "";
    newInteraction.deadline = "";
  }
}

/** Keep table sorted by deadline desc. always.*/
watch( // TODO to watchPostEffect?
  interactions, // TODO Is lodash.cloneDeep required to wath deeply nested props, in arrays?
  () => interactions.value.sort((a, b) => a.deadline - b.deadline),
  { immediate: true, deep: true }
);
</script>

<style scoped>
table {
  table-layout: auto;
  border-spacing: 0px;
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
.form-container {
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

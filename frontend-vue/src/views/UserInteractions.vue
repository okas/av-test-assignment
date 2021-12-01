// eslint-disable-next-line

<template>
  <article class="user-interaction">
    <header>
      <h1>Kasutajate pöördumiste vaade</h1>
    </header>
    <section>
      <div class="form-container">
        <div class="control">
          <input type="text" id="new-description" placeholder="kirjeldus" size="50" v-model="newInteraction.description" />
        </div>
        <div class="control">
          <input id="new-deadline" type="datetime-local" v-model="newInteraction.deadline" />
          <label for="new-deadline"> : tähtaeg</label>
        </div>
        <div class="control">
          <button @click="addNewInteraction(newInteraction)">lisa uus</button>
        </div>
        <div class="control">
          <button class="refresh-icon" @click="getInteractions">↻</button>
        </div>
      </div>
    </section>
    <section>
      <header>
        <h4>Aktiivsed pöördumised</h4>
      </header>
      <table>
        <thead>
          <tr>
            <th>Kirjeldus</th>
            <th>Sisestatud</th>
            <th>Tähtaeg</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in interactions" :key="item.id" :class="{ 'due-problem': isProblematic(item) }">
            <td class="item-description">{{ item.description }}</td>
            <td>{{ formatDateTimeShortDateShortTime(item.created) }}</td>
            <td>{{ formatDateTimeShortDateShortTime(item.deadline) }}</td>
            <td class="item-open">
              <button id="checkbox" @click="markInteractionClosed(item)">✔</button>
            </td>
          </tr>
        </tbody>
      </table>
    </section>
  </article>
</template>

<script>
import formatDateMixin from "../mixins/formatDateMixin";

export default {
  name: "UserInteractions",
  mixins: [formatDateMixin],
  data: () => ({
    interactions: [],
    newInteraction: { description: "", deadline: "" },
  }),
  beforeMount() {
    this.getInteractions();
  },
  methods: {
    async getInteractions() {
      const resp = await this.$api.then((client) =>
        client.execute({
          operationId: "get_api_userinteractions",
          parameters: { isOpen: true },
        })
      );
      if (resp.ok) {
        this.interactions = resp.body.map(this.convertToVM);
      }
    },
    async markInteractionClosed(itnteraction) {
      if (!confirm("Kinnita pöördumise sulgemine")) {
        return;
      }
      const resp = await this.$api.then((client) =>
        client.execute({
          operationId: "patch_api_userinteractions__id_",
          parameters: { id: itnteraction.id },
          requestBody: { id: itnteraction.id, isOpen: false },
        })
      );
      if (resp.ok) {
        this.interactions.splice(this.interactions.indexOf(itnteraction), 1);
      }
    },
    /** Convert dates to JS Date instances from response. */
    convertToVM({ created, deadline, ...rest }) {
      return {
        created: new Date(created),
        deadline: new Date(deadline),
        ...rest,
      };
    },
    isProblematic(interaction) {
      let toCompare = new Date();
      toCompare.setHours(toCompare.getHours() + 1);
      return interaction.deadline < toCompare;
    },
    toYesNo(interaction) {
      return interaction.isOpen ? "jah" : "ei";
    },
    async addNewInteraction({ description, deadline }) {
      const resp = await this.$api.then((client) =>
        client.execute({
          operationId: "post_api_userinteractions",
          requestBody: { description, deadline: new Date(deadline) },
        })
      );
      if (resp.ok) {
        this.interactions.push(this.convertToVM(resp.body));
        this.newInteraction.description = "";
        this.newInteraction.deadline = "";
      }
    },
  },
  watch: {
    /** Keep table sorted by deadline desc. always.*/
    interactions: {
      immediate: true,
      deep: true,
      /** Sort by deadline property/column */
      handler() {
        this.interactions.sort((a, b) => a.deadline - b.deadline);
      },
    },
  },
};
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

.item-open {
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

<template>
  <article class="user-interaction">
    <header>
      <h1>Kasutajate pöördumiste vaade</h1>
    </header>
    <section>
      <header>
        <h2></h2>
      </header>
      <table>
        <thead>
          <tr>
            <th>Kirjeldus</th>
            <th>Sisestatud</th>
            <th>Tähtaeg</th>
            <th>Avatud</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in interactions" :key="item.id">
            <td class="item-description">{{ item.description }}</td>
            <td>{{ formatDateTimeShortDateShortTime(item.created) }}</td>
            <td>{{ formatDateTimeShortDateShortTime(item.deadline) }}</td>
            <td>{{ item.isOpen }}</td>
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
  data: () => ({ interactions: [] }),
  beforeMount() {
    this.getInteractions();
  },
  methods: {
    async getInteractions() {
      const resp = await this.$api.then((client) =>
        client.execute({ operationId: "get_api_userinteractions" })
      );
      if (resp.ok) {
        this.interactions = resp.body;
      }
    },
  },
  watch: {
    "this.interactions"() {
      // Keep table sorted by deadline always.
      this.interactions.sort((a, b) => a.deadline - b.deadline);
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
  td, th {
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
  td.item-description {
    text-align: justify;
  }
</style>

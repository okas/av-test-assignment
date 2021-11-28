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
            <th>Sisestatud</th>
            <th>Tähtaeg</th>
            <th>Kirjeldus</th>
            <th>Avatud</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in interactions" :key="item.id">
            <td class="item">{{ formatDate(item.created) }}</td>
            <td class="item">{{ formatDate(item.deadline) }}</td>
            <td class="item">{{ item.description }}</td>
            <td class="item">{{ item.isOpen }}</td>
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

<style scoped></style>

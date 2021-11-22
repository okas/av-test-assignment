export default {
  methods: {
    /**
     * Format date using Intl API.
     * @param {string|Number|Date} date Any Date constructor compatible parameter.
     * @param {string|string[]} [locale=navigator.languages] Defaults to browser's language list.
     */
    formatDate(date, locale = navigator.languages) {
      const d = new Date(date);
      return new Intl.DateTimeFormat(locale, { dateStyle: "short" }).format(d);
    },
  },
};

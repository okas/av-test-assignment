export default {
  methods: {
    /**
     * Format date using Intl API.
     * @param {string|Number|Date} date Any Date constructor compatible parameter.
     * @param {string|string[]} [locale=navigator.languages] Defaults to browser's language list.
     */
    formatDateShort(date, locale = navigator.languages) {
      const d = new Date(date);
      if (d == "Invalid Date") {
        return d;
      }
      const options = { dateStyle: "short" };
      return new Intl.DateTimeFormat(locale, options).format(d);
    },

    /**
     * Format date using Intl API.
     * Example: 12. dec 2021 HH:mm
     * @param {string|Number|Date} date Any Date constructor compatible parameter.
     * @param {string|string[]} [locale=navigator.languages] Defaults to browser's language list.
     */
    formatDateTimeShortDateShortTime(date, locale = navigator.languages) {
      const d = new Date(date);
      if (d == "Invalid Date") {
        return d;
      }
      const options = {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
      };
      return new Intl.DateTimeFormat(locale, options).format(d);
    },
  },
};

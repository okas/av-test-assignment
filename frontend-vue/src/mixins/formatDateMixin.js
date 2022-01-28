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
     * Format date using Intl API, that respects privided or default locale.
     * Example: 12. dec 2021 15:45 or 12. dec 2021 3:45 PM 
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

    /**
   * Format date using Intl API to "yyyy-MM-ddThh:mm".
   * [HTML DateTime string]{@link https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/datetime-local#value}
   * @param {string|Number|Date} date Any Date constructor compatible parameter.
   */
    formatToHtmlStringDateTime(date) {
      const d = new Date(date);
      if (d == "Invalid Date") {
        return d;
      }
      const isoString = d.toISOString();
      return isoString.substring(0, (isoString.indexOf("T") | 0) + 6 | 0);
    },
  },
};

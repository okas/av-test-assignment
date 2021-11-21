export default {
  methods: {
    formatDate(date, locale) {
      const d = new Date(date);
      return new Intl.DateTimeFormat(locale, { dateStyle: 'short' }).format(d);
    }
  }
}

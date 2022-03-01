export default function useFormatDateTime() {
  function isDate(value) {
    return typeof new Date(value) === "object";
  }

  /**
   * Format date using Intl API.
   * @param date Any Date constructor compatible parameter.
   * @param locale Defaults to browser's language list.
   */
  function formatDateShort(
    date: string | Number | Date,
    locale: string | readonly string[] = navigator.languages
  ): Date {
    const d = new Date(date);
    if (!isDate(date)) {
      return d;
    }
    const options = { dateStyle: "short" };
    return new Intl.DateTimeFormat(locale, options).format(d);
  }

  /**
   * Format date using Intl API, that respects provided or default locale.
   * Example: 12. dec 2021 15:45 or 12. dec 2021 3:45 PM
   * @param date Any Date constructor compatible parameter.
   * @param locale Defaults to browser's language list.
   */
  function formatDateTimeShortDateShortTime(
    date: string | Number | Date,
    locale: string | readonly string[] = navigator.languages
  ): Date {
    const d = new Date(date);
    if (!isDate(date)) {
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
  }

  /**
   * Format date using Intl API to "yyyy-MM-ddThh:mm".
   * [HTML DateTime string]{@link https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/datetime-local#value}
   * @param date Any Date constructor compatible parameter.
   */
  function formatToHtmlStringDateTime(date: string | Number | Date): string {
    const d = new Date(date);
    if (!isDate(date)) {
      return d;
    }
    const isoString = d.toISOString();
    return isoString.substring(0, ((isoString.indexOf("T") | 0) + 6) | 0);
  }

  return {
    formatDateShort,
    formatDateTimeShortDateShortTime,
    formatToHtmlStringDateTime,
  };
}

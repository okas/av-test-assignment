/**
 * "Useless" string is one, that is either `undefined` or undefined, `null` or null, "" or whitespace only.
 * @param {string} test String to test
 */
export function isUselessString(test) {
  return !test?.trim() || test === "undefined" || test === "null";
}

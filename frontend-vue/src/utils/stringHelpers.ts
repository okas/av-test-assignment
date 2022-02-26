/**
 * "Useless" string is one, that is either `undefined` or undefined, `null` or null, "" or whitespace only.
 * @param test String to test
 */
export function isUselessString(test: string | undefined | null) {
  return !test?.trim() || test === "undefined" || test === "null";
}

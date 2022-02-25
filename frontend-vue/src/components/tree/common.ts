export function sanitizeTag(key: string) {
  const underscoreIndex = key.indexOf("_");
  const end = underscoreIndex > 0 ? underscoreIndex : key.length;
  return key.substring(0, end);
}

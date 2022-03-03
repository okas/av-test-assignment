export function useRemoveOne<T>(
  array: Array<T>,
  predicate: // eslint-disable-next-line no-unused-vars
  (arg: T) => boolean
) {
  const itemIndex = array.findIndex(predicate);
  return itemIndex > -1 ? array.splice(itemIndex, 1) : [];
}

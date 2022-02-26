export function useGenerateComponentPath(component: string) {
  const start = component.startsWith("../") ? 2 : 0;
  const end = component.endsWith(".vue")
    ? component.length - 4
    : component.length;
  return component.substring(start, end);
}

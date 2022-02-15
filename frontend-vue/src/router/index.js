import { createRouter, createWebHistory } from "vue-router";
import { rawRouteOptions } from "./routes";

// Apply dynamic loading for all the routes
rawRouteOptions.forEach((route) => {
  let viewPath = route.component;
  const start = viewPath.startsWith("../") ? 2 : 0;
  const end = viewPath.endsWith(".vue") ? viewPath.length - 4 : viewPath.length;
  viewPath = viewPath.substring(start, end);
  // Start and extension must be static strings for Rollup dynamic import!
  route.component = () => import(`../${viewPath}.vue`);
});

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: rawRouteOptions,
});

export default router;

import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import { useGenerateComponentPath } from "./helpers";
import { rawRouteOptions } from "./routes";

// Apply dynamic loading for all the routes
const routes: RouteRecordRaw[] = rawRouteOptions.map(
  ({ component, ...rest }) => {
    const viewPath = useGenerateComponentPath(component);
    return {
      component: () => import(`../${viewPath}.vue`), // Start and extension must be static strings for Rollup dynamic import!
      ...rest,
    };
  }
);

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;

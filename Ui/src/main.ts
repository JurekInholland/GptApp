import { createPinia } from "pinia";
import { createPersistedState } from "pinia-plugin-persistedstate";
import router from "./router";
import { createApp } from "vue";
import { autoAnimatePlugin } from "@formkit/auto-animate/vue";

import App from "./App.vue";
import { VueSignalR } from "@quangdao/vue-signalr";
// import PerfectScrollbar from "vue3-perfect-scrollbar";
// import "vue3-perfect-scrollbar/dist/vue3-perfect-scrollbar.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/700.css";
import "@fontsource/roboto-mono/400.css";
import "@fontsource/roboto-mono/700.css";
// import "input-range-scss/_inputrange.scss";

import CustomScrollbar from "custom-vue-scrollbar";
import "custom-vue-scrollbar/dist/style.css";
import "./assets/main.css";

const pinia = createPinia();
pinia.use(
  createPersistedState({
    auto: true,
  })
);

const app = createApp(App)
  .use(pinia)
  .use(router)
  .use(VueSignalR, { url: "/api/signalr" })
  .use(autoAnimatePlugin);
//   .use(PerfectScrollbar, {
//     watchOptions: true,
//   })
app.component(CustomScrollbar.name, CustomScrollbar);

declare module "vue" {
  export interface GlobalComponents {
    CustomScrollbar: typeof CustomScrollbar;
  }
}

app.mount("#app");

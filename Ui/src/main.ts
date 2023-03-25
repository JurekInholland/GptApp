import { createPinia } from "pinia";
import router from "./router";
import { createApp } from "vue";
import "./assets/main.css";
import App from "./App.vue";
import { VueSignalR } from "@quangdao/vue-signalr";
// import PerfectScrollbar from "vue3-perfect-scrollbar";
// import "vue3-perfect-scrollbar/dist/vue3-perfect-scrollbar.css";
import "@fontsource/roboto";
import "@fontsource/roboto-mono";
createApp(App)
  .use(createPinia())
  .use(router)
  .use(VueSignalR, { url: "/api/signalr" })
  //   .use(PerfectScrollbar, {
  //     watchOptions: true,
  //   })

  .mount("#app");

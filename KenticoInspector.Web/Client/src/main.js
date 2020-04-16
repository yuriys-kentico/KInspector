import "@babel/polyfill";
import Vue from "vue";
import VueShowdown from "vue-showdown";
import "./plugins/vuetify";
import App from "./frontend/app";
import router from "./router";
import store from "./store";
import "roboto-fontface/css/roboto/roboto-fontface.css";
import "@mdi/font/css/materialdesignicons.css";
import vuetify from "@/plugins/vuetify";

Vue.config.productionTip = false;

Vue.use(VueShowdown,
  {
    options: {
      openLinksInNewWindow: true
    }
  });

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount("#app");
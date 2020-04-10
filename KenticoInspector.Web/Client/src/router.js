import Vue from 'vue';
import Router from 'vue-router';
import Home from './frontend/home/home';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/connect',
      name: 'connect',
      component: () => import('./frontend/connect/connect')
    },
    {
      path: '/connect/:instanceGuid/reports',
      name: 'reports',
      component: () => import('./frontend/reports/reports'),
      props: true
    },
    {
      path: '/connect/:instanceGuid/actions',
      name: 'actions',
      component: () => import('./frontend/actions/actions'),
      props: true
    }
  ]
});
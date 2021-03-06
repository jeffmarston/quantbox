import Vue from 'vue';
import Router from 'vue-router';

// Containers
const DefaultContainer = () => import('@/containers/DefaultContainer');

// Views - Components
const CodeEditor = () => import('@/views/dashboard/CodeEditor');
const SplitPanel = () => import('@/containers/SplitPanel');
const Settings = () => import('@/views/general/Settings');

// Views - Pages
const Page404 = () => import('@/views/pages/Page404');
const Login = () => import('@/views/pages/Login');
const _ = require('lodash');

// import Vue from 'vue'
// import VueCodemirror from 'vue-codemirror'
// import 'codemirror/lib/codemirror.css'

// Vue.use(VueCodemirror,  { 
//   options: { theme: 'base16-dark' },
//   events: ['scroll']
// })

Vue.use(Router)

const router = new Router({
  mode: 'hash', // https://router.vuejs.org/api/#mode
  linkActiveClass: 'open active',
  scrollBehavior: () => ({ y: 0 }),
  routes: []
})

router.addRoutes([
    {
      path: '/',
      redirect: '/dashboard',
      name: 'Home',
      component: DefaultContainer,
      children: [
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: SplitPanel
        },
        {
          path: 'algos',
          name: 'algorithms',
          redirect: '/algos/algo1',
          component: {
            render(c) { return c('router-view') }
          },
          children: [
            {
              path: 'algo1',
              name: 'algo1',
              component: CodeEditor
            },
            {
              path: 'algo2',
              name: 'algo2',
              component: CodeEditor
            },
            {
              path: 'algo3',
              name: 'algo3',
              component: CodeEditor
            }
          ]
        },
        {
          path: 'settings',
          name: 'Settings',
          component: Settings
        }
      ]
    },
    {
      path: '/pages',
      redirect: '/pages/404',
      name: 'Pages',
      component: {
        render(c) { return c('router-view') }
      },
      children: [
        {
          path: '404',
          name: 'Page404',
          component: Page404
        },
        {
          path: 'login',
          name: 'Login',
          component: Login
        }
      ]
    }
  ]);


export default router;

import Vue from 'vue';
import Router from 'vue-router';

// Containers
const DefaultContainer = () => import('@/containers/DefaultContainer');

// Views - Components
const ClientOverview = () => import('@/views/ClientOverview');
const CodeDiff = () => import('@/views/CodeDiff');
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
      redirect: '/overview',
      name: 'Home',
      component: DefaultContainer,
      children: [
        {
          path: 'overview',
          name: 'Overview',
          component: ClientOverview
        },
        {
          path: 'client',
          name: 'client',
          component: ClientOverview
        },
        {
          path: 'diff',
          name: 'diff',
          component: CodeDiff
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

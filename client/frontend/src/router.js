import { createRouter, createWebHistory} from 'vue-router'

import Home from './views/Home.vue'
import MyUrl from'./views/MyUrl.vue'
import Login from'./views/Login.vue'

const routes = [
{
    path: '/',
    redirect: '/home'
},
{
    path: '/home',
    name: 'Home',
    component: Home
},
{
    path: '/login',
    name: 'Login',
    component: Login
},
{
    path: '/myurl',
    name: 'MyUrl',
    component: MyUrl
}


]
const router = createRouter({
    history: createWebHistory(),
    routes: routes 
})

export default router
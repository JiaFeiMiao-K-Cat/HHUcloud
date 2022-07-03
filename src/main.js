import { createApp } from 'vue'
import App from './App.vue'
import './assets/index.css'
import router from './router/index.js'
import request from './api/request';
import store from './store'

import ElementUI from 'element-plus'
import 'element-plus/dist/index.css'



request.defaults.baseURL = '/api'


const app = createApp(App)
app.use(router)
app.use(store)
app.use(ElementUI)
app.mount('#app')


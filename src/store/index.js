import { createApp } from 'vue'
import App from '../App.vue'
import { createStore } from 'vuex'

import login from './login'
import register from './register'
import top_list from './top_list'
import search from './search'
import recent_songs from './recent_songs'
import my_song_lists from './my_song_lists'
import footer from './footer'
import header from './header'
import user from './user'


// Create a new store instance.
const store = createStore({
  modules:{
    login,
    register,
    top_list,
    search,
    recent_songs,
    my_song_lists,
    footer,
    header,
    user
  }
})

const app = createApp(App)

// Install the store instance as a plugin
app.use(store)

export default store
// 配置路由

import { createRouter, createWebHashHistory } from "vue-router"

import login from '../pages/login'
import register from '../pages/register'
import top_list from '../pages/top_list'
import search_result from '../pages/search_result'
import my_song_lists from '../pages/my_song_lists'
import recent_songs from '../pages/recent_songs'
import user from '../pages/user'

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: "/home",
      component: top_list,
      // show作为路由页面的属性用于底部的显示与隐藏
      meta:{show:true}
    },
    {
      path: "/login",
      component: login,
      meta:{show:false}
    },
    {
      path: '/register',
      component: register,
      meta:{show:false}
    },
    {
      // :keyword用于占位，传递用户输入的参数(params)
      // keyword后无？代表必须传递，若不传递浏览器url会出错，有？则可不传该参数
      path: "/search/:keyword",
      component: search_result,
      meta:{show:true},
      name:"search",
      // 路由组件也可传递props参数
      // 布尔值写法：传的是params参数
      // props:true
      // 对象值写法:用于给路由组件额外传递参数
      // props:{
      //   a:1,
      //   b:2,
      // }
      // 函数写法：可以对params和query参数进行处理后传到页面
      // props:($route)=>{
      //   return {keyword:$route.params.keyword,k:$route.query.k};
      // }
    },
    {
      path: "/my_song_lists",
      component: my_song_lists,
      meta:{show:true}
    },
    {
      path: "/recent_songs",
      component: recent_songs,
      meta:{show:true}
    },
    {
      path: "/",
      redirect: "/home",
      meta:{show:true}
    },
    {
      path:'/user',
      component: user,
      meta:{show:true}
    }
  ]
})

export default router
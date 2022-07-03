<template>
  <div class="bg">
    <!-- 头部 -->
    <div class="header">
      <!-- 顶部 -->
      <div class="top">
        <!-- 顶部左侧 -->
        <div class="topl">
          <router-link to="/home">
            <a href="#" class="LOGO">
              <img class="logo_image" src="../../assets/LOGO.png">
              <span class="logo_name">Emo云音乐</span>
            </a>
          </router-link>
        </div>
        <!-- 顶部右侧 -->
        <div class="topr">
          <!-- 搜索 -->
          <a href="#" class="search">
            <input class="searchtxt" type="text" v-model="inputValue" @keyup.enter="toSearch()" placeholder="请输入歌曲名">
            <i><img src="../../assets/search.png" @click.enter="toSearch"></i>
          </a>
          <!-- 用户 -->
          <a href="#" class="user">
            <span class="user_image"><img src="../../assets/user.png"></span>
            <span class="user_name" >{{ userName }}</span>
            <router-link to="/login"><span class="login">登录</span></router-link>
            <router-link to="/register"><span class="login">注册</span></router-link>
          </a>
        </div>
      </div>

      <!-- 导航 -->
      <div class="nav">
        <ol class="nav_items w">
          <a href="#">
            <router-link to="/">
              <li class="nav_choose" @click.enter="toTopList">热门歌曲</li>
            </router-link>
          </a>
          <a href="#">
            <router-link to="/my_song_lists">
              <li>我的歌单</li>
            </router-link>
          </a>
          <a href="#">
            <router-link to="/recent_songs">
              <li>最近播放</li>
            </router-link>
          </a>
          <a href="#">
            <router-link to="/user">
              <li>个人信息</li>
            </router-link>
          </a>
        </ol>
      </div>
    </div>

    <!-- 主体 -->
    <div class="main">
      <div class="message">
        <!-- 路由出口 -->
        <router-view></router-view>
      </div>
      <div class="occupied"></div>
    </div>

  </div>


</template>


<script>

import { mapState } from 'vuex';
export default {
  data() {
    return {
      inputValue:'',
      
    }
  },
  computed: {
    ...mapState(
      {
        userName: (state) => {
          return state.login.userInfo.name
        }
      }
    ),
  },
  components: {

  },
  created() {


  },
  mounted(){
    this.$store.dispatch('getSongInList')
  },
  methods: {

    // 搜索回调函数，向search跳转-编程式导航
    toSearch() {
      // params
      // this.$router.push('/search/' + this.inputValue)

      // query
      // this.$router.push('/search/' + this.inputValue +"?k="+this.inputValue.toUpperCase())

      // 对象(该方式不能用路径形式，需要给页面设置name属性)
      this.$router.push({
        name:'search',
        params:{
          // undefined用于传递空串时仍然跳转到search路由
          // keyword:this.inputValue||undefined,
          keyword:this.inputValue
        },
        query:{
          K:this.inputValue.toUpperCase(),
        }
      })
    },


    



  }
}
</script>

<style>
.bg {
  position: absolute;
  width: 100%;
  height: 100%;
  background-color: #FFFEE6;
}

/* 头部样式 */
.header {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100px;
  background-color: #80BD64;
}

/* 顶部样式 */
.top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 75px;
  padding: 0 120px;
  color: white;
}

/* 顶部左侧 */
.LOGO {
  height: 50px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo_image {
  display: inline-block;
  width: 35px;
  height: 35px;
  padding-right: 10px
}

.logo_name {
  height: 50px;
  font-size: 20px;
  line-height: 50px;
  color: white;
}

/* 顶部右侧 */
.topr {
  display: flex;
  align-items: center;
}

/* 搜索 */
.search {
  display: flex;
  align-items: center;
}

.searchtxt {
  display: inline-block;
  width: 300px;
  height: 30px;
  border: 1px solid #5c9141;
  border-radius: 13px;
  background-color: white;
  padding: 0 13px;
}

.search i img {
  width: 30px;
  height: 30px;
  margin-left: 10px;
  margin-right: 30px;
}

/* 用户 */
.user {
  display: flex;
  align-items: center;
}


.user_image {
  height: 40px;
  border: 2px solid #85b36e;
  border-radius: 40px;
  background-color: white;
  margin: 0 10px;
}

.user_image img {
  width: 40px;
  height: 40px;
}

.user_name {
  color: white;
}

.login {
  margin-left:10px;
}

.login,.register {
  width: 35px;
  text-align: center;
  color: white;
}


/* 导航 */
.nav {
  display: flex;
  justify-content: space-around;
  align-items: center;
  height: 30px;
  background-color: #a3cb90;
  border-bottom: 1px solid rgba(207, 207, 207, 0.384);
}

.nav_items {
  height: 25px;
  width: 1200px;
  display: flex;
  justify-content: space-around;
  align-items: center;
}

.nav_items a li {
  height: 25px;
  color: #5c9141;
  font-size: 12px;
  line-height: 25px;
  padding: 0 50px;
}

.nav_items .nav_choose {

  background-color: #a3cb90;
  border-radius: 0 0 5px 5px;
}
</style>
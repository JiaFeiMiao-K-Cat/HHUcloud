<template>
  <div>
    <Header></Header>
    <!-- 主体 -->
    <div class="main">
      <div class="message">
        <!-- 路由出口 -->
        <router-view></router-view>
      </div>
      <div class="occupied"></div>
    </div>
    <Footer v-show="$route.meta.show"></Footer>
  </div>
</template>

<script>
import Header from './components/Header'
import Footer from './components/Footer'
export default {
  name: 'App',
  components: {
    Header,
    Footer
  }
}
</script>

<style>

/* 主体样式 */
.main {
  padding-top: 105px;
  height: fit-content;
  left: 0px;
  right: 0px;
  width: 100%;
  background-color: #FFFEE6;
}

.message {

  margin: 0 200px;
  padding: 10px;
  height: fit-content;
  border-left: 1px solid #EBEBCC;
  border-right: 1px solid #EBEBCC;
  background-color: #FFFCD9;
  border-radius: 0 0 10px 10px;
}

.occupied {
  height: 110px;
}
</style>

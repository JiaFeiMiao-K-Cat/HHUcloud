<template>

  <!-- 轮播图 -->
  <el-carousel :interval="4000" type="card" height="200px">
    <el-carousel-item v-for="(item, index) in songList.slice(0,5)" :key="index" @click="playMusic('https://music.163.com/outchain/player?type=2&id='+item.id+'&auto=1&height=66')">
      <a href="#">
        <img :src="item.imgLink" alt=" ">
      </a>
    </el-carousel-item>
  </el-carousel>

  <div class="null"></div>

  <!-- 榜单歌曲 -->
  <div class="items">
    <div class="item" v-for="(item, index) in songList" :key="index">
      <a href="#">
        <img :src=item.imgLink alt=" " @click="playMusic('https://music.163.com/outchain/player?type=2&id='+item.id+'&auto=1&height=66')">
        <p>{{ item.title }}-{{ item.artistId }}</p>
      </a>
    </div>

  </div>



</template>

<script>

import { mapState } from 'vuex'
export default {
  name: 'top_list',
  computed: {
    ...mapState({
      songList: (state) => {
        return state.top_list.songList
      }
    })
  },
  created() {

  },
  // 组件挂载完毕，向服务器发送请求
  mounted() {
    this.$store.dispatch('songs')

    this.$store.dispatch('getUserInfo')
  },
  components: {

  },
  methods: {
    playMusic(url) {
      console.log(url)
      this.$store.state.footer.musicUrl=url
      // this.$store.dispatch('addPlayTimes')
    },
    getSongerName(songID){
      this.$store.dispatch('get_SongerName',songID)
    },
    
    
  }
}
</script>






<style scoped>
/* 轮播图样式 */

.el-carousel__item {
  text-align: center;
}
.el-carousel__item img {
  height: 200px;
  width: 300px;
  color: #cce4a5;
  opacity: 0.75;
  line-height: 200px;
  margin: 0;
  text-align: center;
}

.el-carousel__item:nth-child(2n) {
  background-color: #c1d8c580;
}

.el-carousel__item:nth-child(2n + 1) {
  background-color: #e2f6c636;
}
</style>

<style>
/* 占位 */
.null {
  height: 1px;

}

/* 热播歌曲样式 */
.items {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-around;
}

.item {
  display: inline-block;
  width: 110px;
  height: 130px;
  overflow: hidden;
  background-color: #d4e5b8;
  margin: 10px 0;
}

.item a {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.item a img {
  width: 110px;
  height: 110px;
  margin: 0 auto;
}

.item p {
  width: 100px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  text-align: center;
}

/* .item div img {
  width: 110px;
  height: 110px;
  margin: 0 auto;
} */
/* .item .srtImg {
  background-color: rgba(105, 105, 105, 0.2);
}
.item .srtImg img{
  width: 110px;
  height: 110px;
  margin: 0 auto;
} */
</style>
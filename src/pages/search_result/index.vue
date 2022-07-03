<template>
  <div class="result_header">
    <div class="result">{{ $route.params.keyword }} 的搜索结果：</div>

    <ol class="header_items">
      <li class="listID">序号</li>
      <li class="songName">标题</li>
      <li class="songerName">歌手</li>
      <li class="timeLong">时长</li>
      <li class="addMyList">加入歌单</li>
    </ol>

    <div class="result_content" v-for="(item, index) in searchList" :key="index++" @click="playMusic('https://music.163.com/outchain/player?type=2&id='+item.id+'&auto=1&height=66')">
      <ol class="song_item">
        <li class="listID">{{ index }}</li>
        <li class="songName">{{ item.title }}</li>
        <li class="songerName">{{ item.artistId }}</li>
        <li class="timeLong">{{ item.duration}}</li>
        <li class="addMyList" @click="reqAddSongsToList('1',item.id)">+</li>
      </ol>
    </div>


  </div>
</template>

<script>

import { mapState } from 'vuex'
export default {
  name: 'search',
  computed: {
    ...mapState({
      searchList: (state) => {
        return state.search.searchList
      }
    }),
    ...mapState({
      playlistId: (state) => {
        return state.search.uerInfo.playLists
      }
    }),
  },
  data() {
    return {
      // 路由布尔类props传递param参数
      props:['keyword']
      // 路由对象类型props可传递自定义额外参数
      // props:{
      //   a:1,
      //   b:2,
      // }
      // 函数写法
      // props:{
      //   'keyword',
      //   'k'
      // }
    }
  },
  created() {
    this.$store.dispatch('getSearchList', this.$route.params.keyword)
  },
  mounted() {
    
  },
  methods: {
    playMusic(url) {
      this.$store.state.footer.musicUrl=url
      this.$store.dispatch('addPlayTimes')
    },
    // 添加歌曲
    reqAddSongsToList(playlistId,songId){
      this.$store.dispatch('AddSongsToList',{playlistId,songId})
    },
    
  }
}
</script>

<style>
.result {
  height: 20px;
  background-color: #95cd804f;
  padding: 5px 20px;
  color: rgb(41, 82, 22);
  border-radius: 10px 10px 0 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.result_header {
  width: 100%;
  padding-bottom: 10px;
  background-color: #c8eab89c;
  border-radius: 10px;
}

.header_items {
  display: flex;
  justify-content: space-around;
  padding: 5px 0;
  color: rgb(162, 170, 154);
  cursor: pointer;
}

.song_item {
  display: flex;
  justify-content: space-around;
  padding: 5px 0;
  color: #357735;
  cursor: pointer;
}

.listID {
  width: 50px;
  text-align: center;
  background-color: rgb(241, 255, 221);
}

.songName {
  width: 250px;
  text-align: center;
  background-color: rgb(241, 255, 221);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.songerName {
  width: 200px;
  text-align: center;
  background-color: rgb(241, 255, 221);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.timeLong {
  width: 100px;
  text-align: center;
  background-color: rgb(241, 255, 221);
}

.addMyList {
  width: 60px;
  text-align: center;
  background-color: rgb(241, 255, 221);
}
</style>
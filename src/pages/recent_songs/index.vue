<template>
  <div class="result_header">
    <div class="result">{{ userName }} 最近听的歌：</div>

    <ol class="header_items">
      <li class="listID">序号</li>
      <li class="songName">标题</li>
      <li class="songerName">歌手</li>
      <li class="timeLong">时长</li>
      <li class="addMyList">加入歌单</li>
    </ol>


    <div class="result_content" v-for="(items, index) in recent_songs" :key="index" @click="playMusic(item.url)">
      <ol class="song_item">
        <li class="nlistID">{{ index }}</li>
        <li class="songName">xxxx</li>
        <li class="songerName">fff</li>
        <li class="timeLong">11:11</li>
        <li class="addMyList" @click="addSong(item.ID)">+</li>
      </ol>
    </div>
  </div>


</template>

<script>

import { mapState } from 'vuex'

export default {
  name: 'my_song_lists',

  data() {

  },
  mounted() {

  },
  computed: {
    ...mapState({
      userInfo: (state) => {
        return state.login.userInfo
      }
    })
  },
  components: {

  },
  creaded() {

  },
  computed: {
    ...mapState({
      recent_songs: (state) => {
        return state.top_list.userInfo.historyPlay
      }
    })
  },
  methods: {
    addSong(){
      
    }
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
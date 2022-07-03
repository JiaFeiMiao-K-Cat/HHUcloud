<template>
  <div class="my_song_lists">
    <div class="result">{{ userName }} 的歌单：</div>

    <ol class="header_items">
      <li class="listID">序号</li>
      <li class="songName">标题</li>

      <li class="timeLong">时长</li>
      <li class="addMyList" @click="delSong(item.ID)">删除</li>
    </ol>


    <div class="result_content1" v-for="(item, index)  in songList" :key="index" @click="playMusic('https://music.163.com/outchain/player?type=2&id='+item.mySong.id+'&auto=1&height=66')">
      <div v-if="index > 0">
        <ol class="song_item">
          <li class="listID">{{ index }}</li>
          <li class="songName">{{ item.mySong.title }}</li>

          <li class="timeLong">{{ item.mySong.duration }}</li>
          <li class="addMyList">-</li>
        </ol>
      </div>
    </div>


  </div>
</template>

<script>

import { mapState } from 'vuex'
export default {
  name: 'my_song_lists',
  data() {
    return {

    }
  },
  components: {

  },
  beforeCreate() {
    // this.$store.dispatch('getSongInList')
  },
  creaded() {

  },
  mounted() {


  },
  computed: {
    ...mapState({
      songList: (state) => {
        return state.my_song_lists.mySongs
      }
    }),
    ...mapState(
      {
        userName: (state) => {
          return state.login.userInfo.name
        }
      }
    ),

  },
  methods: {

    playMusic(url) {
      this.$store.state.footer.musicUrl=url

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

.my_song_lists {
  width: 100%;
  padding-bottom: 10px;
  background-color: #c8eab89c;
  border-radius: 10px;
}

.my_song_lists .header_items {
  display: flex;
  justify-content: space-around;
  padding: 5px 0;
  color: rgb(162, 170, 154);
  cursor: pointer;
}

.my_song_lists .song_item {
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

.my_song_lists .songName {
  width: 350px;
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
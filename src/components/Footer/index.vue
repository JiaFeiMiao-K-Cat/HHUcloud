<template>
  <!-- 底部播放栏
  <div class="player">
    <audio :src="musicUrl" controls></audio>
  </div> -->

  <iframe id="inlineFrameExample"
    class="player"
    title="Inline Frame Example"
    width="300"
    height="200"
    :src=musicUrl >
  </iframe>
</template>




<script>

// 导入页面


import { mapState } from 'vuex'

export default {
  data() {
    return {

    }
  },
  computed: {
    ...mapState({
      musicUrl: (state) => {
        return state.footer.musicUrl
      }
    })
  },
  components: {

  },
  created() {


  },
  methods: {

  }
}
</script>

<style>
/* 底部样式 */

.player {
  position: fixed;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 80px;
  border-radius: 15px 15px 0 0;
  background-color: #acacac0e;
  color:#bbc4b617;

}
</style>
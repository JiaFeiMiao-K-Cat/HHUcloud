import { reqGetSongInList, reqGetSongByID } from "@/api";

const state = {
  mySongs: [{ 'mySong': {'id':'-1'} }],
};
const mutations = {
  GETSONGSINLISTS(state, mySong) {

    if (state.mySongs[state.mySongs.length - 1].mySong.id != mySong.id) {
      console.log(state.mySongs[state.mySongs.length - 1])
      state.mySongs.push({ mySong })
    }

  },



};
const actions = {
  async getSongInList({ commit }) {
    let result = await reqGetSongInList()
    if (result.status == 200) {
      for (let i = 0; i < result.data.songList.length; ++i) {
        let result1 = await reqGetSongByID(result.data.songList[i])

        commit('GETSONGSINLISTS', result1.data)
      }
    }

  },
  async reqGetSongByID({ commit }, songID) {
    let result = await reqGetSongByID(songID)
    return result.data
  }

};
const getters = {};

export default {
  state,
  mutations,
  actions,
  getters,
}
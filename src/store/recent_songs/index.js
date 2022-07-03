import { reqGetSongListsInfo } from "@/api";

const state = {
  recentSongs:[]
};
const mutations = {
  RECENTLISTEN(state,recentSongs){
    state.recentSongs = recentSongs
  }
};
const actions = {
  async getUserRecentListen({commit}){
    let result = await getRecentListen()
    if(result.state==200){
      commit('RECENTLISTEN',result.data)
    }
  },
  
};
const getters = {};

export default {
  state,
  mutations,
  actions,
  getters,
}
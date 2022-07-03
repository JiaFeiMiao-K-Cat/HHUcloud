import { getSearchInfo,getSongerName,reqAddSongsToList } from '@/api'

const state = {
  searchList: [],
};
const mutations = {
  async GETSEARCHLIST(state,searchList){
    for(let i=0;i<searchList.length;++i){
      // 获取歌手信息
     let result = await getSongerName(searchList[i].id)
     searchList[i].artistId=result.data[0].item1
   }
   state.searchList=searchList
 }
};
const actions = {
  // 获取播放列表
  async getSearchList({ commit }, keyword) {
    let result = await getSearchInfo(keyword)
    if(result.status == 200) {
      commit('GETSEARCHLIST',result.data)
    }
  },
  // 将歌曲加入歌单
  async AddSongsToList({ commit },addInfo ) {
    let result = await reqAddSongsToList(addInfo.playlistId,addInfo.songId)
    
  },

};
const getters = {};

export default {
  state,
  mutations,
  actions,
  getters,
}
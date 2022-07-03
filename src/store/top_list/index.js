import {topSongs,getSongerName} from '../../api'


const state = {
  songList:[],
  userInfo:{}
};
const mutations = {
  async SONGS(state,categoryList){
     for(let i=0;i<categoryList.length;++i){
      let result = await getSongerName(categoryList[i].id)
      categoryList[i].artistId=result.data[0].item1
    }
    state.songList=categoryList
  },
  GETUSERINFO(state,userInfo){
    state.userInfo=userInfo
  }
  
};
const actions = {

  // 通过api获取歌曲数据
  async songs({commit}) {
    let result = await topSongs()
    if(result.status==200){
      commit("SONGS",result.data)
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
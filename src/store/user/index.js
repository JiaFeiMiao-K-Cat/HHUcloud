import {reqResetUserName} from '../../api'

const state = {

};
const mutations = {
  
};
const actions = {
  async resetUserName({commit},newName){
    
    await reqResetUserName(newName)

    // if(result.status==200 || result.status==201){
    //   commit("GETUSERINFO",result.data)
    // }
  },
};
const getters = {};

export default {
  state,
  mutations,
  actions,
  getters,
}
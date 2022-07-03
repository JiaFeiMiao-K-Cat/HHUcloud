import {getToken,reqGetUserInfo} from "@/api";


const state = {
  userToken:'',
  isLogin:false,
  userInfo:{}
};
const mutations = {
  GETTOKEN(state,userToken){    
    state.userToken=userToken
    state.isLogin=true;
  },
  GETUSERINFO(state,userInfo){
    state.userInfo=userInfo
  }
};
const actions = {
  async getMyToken({commit,state},formData){
    let result = await getToken(formData)
    if(result.status==200){
      state.isLogin=true;
      localStorage.setItem("TOKEN",result.data)
      commit('GETTOKEN',result.data)
    } 
  },

  async getUserInfo({commit}){
    let result = await reqGetUserInfo()
    if(result.status==200 || result.status==201){
      commit("GETUSERINFO",result.data)
    }
  }
};
const getters = {};

export default {
  state,
  mutations,
  actions,
  getters,
}
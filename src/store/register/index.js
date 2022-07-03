import { register } from "@/api";

const state = {
  userInfo:[],
  isRegisted:false,
};
const mutations = {
  USERINFO(state,userInfo){
    state.userInfo=userInfo
  },
  REGISTERR(state){
    state.isRegisted=true
  },
  
};
const actions = {
  async userRegister({commit},user){
    let result = await register(user.email,user.username,user.password)
    if(result.status==200||result.status==201){
      commit('USERINFO',result.data)
    } else{
      commit('REGISTERR')
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
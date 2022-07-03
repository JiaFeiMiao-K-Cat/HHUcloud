// 对axios进行二次封装
import axios from "axios";

import store from '@/store'

// 创建axios实例
const requests = axios.create({
  // 请求超时时间5s
  timeout:5000,
});

// 请求拦截器-在请求发出前进行操作
requests.interceptors.request.use((config)=>{
  // config:配置对象，含有headers请求头
  if(store.state.login.userToken){
    config.headers.Authorization = 'Bearer '+store.state.login.userToken;
  }


  
  return config;
})

// 响应拦截器
requests.interceptors.response.use(
  // // 成功的回调
  // (res)=>{
  //   return res.data;
  // },
  // // 失败的回调
  // (error)=>{
  //   return Promise.reject(new Error('faile'));
  // }
)

// 对外导出
export default requests;



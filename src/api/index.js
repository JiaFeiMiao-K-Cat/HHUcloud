// 对api接口进行统一管理

// 引入封装后的axios
import requests from './request';



// top_list接口
export const topSongs = ()=>{
  return requests({url:'songs/top/35',method:'get'});
}

// search接口
export const getSearchInfo = (params)=>{
  return requests.post('/Songs/Search', '\"' + params + '\"', {headers: { "Content-Type": "application/json" }})
}

// 获取歌手名
export const getSongerName = (songID)=>{
  if(songID!=null){
    return requests({url:'/Songs/Artists/'+songID,method:'get'})
  }
}

// 注册接口
export const register = (email1,name1,password1)=>{
    return requests.post('/Users/Regist', { email: email1, name: name1, password: password1 },  {headers: {'Content-Type': 'multipart/form-data'}}).catch(error=>{
      return 'err'
    })
}

// 获取token
export const getToken = (formData)=>{
  return requests.post('/Token',formData)
}

// 用token获取用户信息
export const reqGetUserInfo = ()=>{
  return requests.post('/Users/Info')
}


// 获取用户歌单
export const reqGetSongListsInfo = ()=> {
  return requests.post('/Users/PlayList')
}

// 获取指定id的播放列表
export const reqGetSongInList = ()=> {
  return requests.get('/PlayLists/'+'1')
}

// 根据id获取歌曲信息
export const reqGetSongByID = (songID)=> {
  return requests.get('/Songs/'+songID)
}

// 向用户歌单添加歌曲
export const reqAddSongsToList = (playlistId,songId)=> {
  return requests.post('/PlayLists/AddSong',{ playlistId: playlistId ,songId:songId},  {headers: {'Content-Type': 'multipart/form-data'}})
}

// 修改用户名
export const reqResetUserName = (newName)=>{
  return requests.post('/Users/ChangeUserName', { newName: newName },  {headers: {'Content-Type': 'multipart/form-data'}})
}






// // 播放次数增加接口
// export const addPlayNum = (songID)=>{
//   return requests({url:'/Songs',method:'post',data:songID})
// }

// // 加入歌单接口
// export const toMyList = (userID,songID)=>{
//   return requests({url:'/toMyList',method:'post',data:{userID,songID}})
// }

// // 从歌单中删除
// export const outMyList = (userID,songID)=>{
//   return requests({url:'/outMyList',method:'post',data:{userID,songID}})
// }

// // 获取用户歌单
// export const getMySongList = ()=>{
//   return requests({url:'/userSongList',method:'get'})
// }

// 最近播放
export const getRecentListen = ()=>{
  return requests({url:'/userRecentListen',method:'get'})
}









<template>
  <div class="userx">

    <ol>
      <span>用户名：{{userInfo.name}}</span>
      <li calss="email"><i>修改用户名：</i><input name="email" type="text" placeholder="请输入用户名" v-model="newName"><button @click="resetUserName()">确认</button></li>
      
      <br>
      <!-- <span>修改用户密码：</span>
      <li class="password"><i>原密码：</i><input name="password" type="password" placeholder="请输入原密码" v-model="password"></li>
      <li class="password"><i>新密码：</i><input name="password" type="password" placeholder="请输入新密码" v-model="password1"></li> -->
    </ol>

  </div>
</template>


<script>

// 导入页面

import { mapState } from 'vuex';

export default {
  name: 'login',
  data() {
    return {
      newName: '',
      password: '',
      password1: '',
    }
  },
  computed: {
    ...mapState(
      {
        userInfo: (state) => {
          return state.login.userInfo
        }
      }
    )
    
  },
  methods: {

    resetUserName(){
      this.$store.dispatch('resetUserName',this.newName)
      
    }

    



  }
}
</script>

<style>
.userx {

  text-align: center;
  left: 280px;
  height: 80px;
  width: 300px;
  background-color: #9fc28d;
  border-radius: 10px;
  padding: 8px 0;
  margin: 30px 0 30px 280px;
}

.userx button {
  margin: 10px;
  display:inline-block;
  background-color: rgb(246, 246, 204);
}

.userx .goBack {
  color: rgb(246, 246, 204);
}

.userx ol {
  display: flex;
  flex-flow: column;
  align-items: center;
  color: rgb(250, 255, 215)
}


.userx ol li {
  padding: 8px 0;
}

.userx ol li i {
  display: inline-block;
  width: 80px;
}

.userx input {
  background-color: rgb(246, 246, 204);
  border: 1px solid #8eb57b;
  padding: 3px;
  border-radius: 4px
}

</style>

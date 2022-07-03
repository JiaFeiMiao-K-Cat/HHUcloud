<template>
  <div class="userlogin">
    <router-link to="/home">
      <div class="goBack">返回</div>
    </router-link>
    <ol>
      <li calss="email"><i>邮箱：</i><input name="email" type="text" placeholder="请输入邮箱" v-model="email"></li>
      <li class="password"><i>密码：</i><input name="password" type="password" placeholder="请输入密码" v-model="password"></li>

    </ol>
    <div class="regButton">
      <button class="regButtonName" @click="doLogin()">登&nbsp;&nbsp;录</button>
      <br>
      <router-link to="/register"><span class="toLogin">还没有账号?去注册</span></router-link>
    </div>
  </div>
</template>


<script>

// 导入页面

import { mapState } from 'vuex';

export default {
  name: 'login',
  data() {
    return {
      email: '',
      password: '',

    }
  },
  computed: {
    ...mapState(
      {
        isLogin: (state) => {
          return state.login.isLogin
        }
      }
    ),
    ...mapState(
      {
        userToken: (state) => {
          return state.login.userToken
        }
      }
    ),
    
  },
  components: {

  },
  Updated() {


  },
  methods: {

    doLogin() {
      
      let email = this.email;
      let password = this.password;
      let formData = new FormData();
      formData.append('email', email);
      formData.append('password', password);
      (email && password) && this.$store.dispatch('getMyToken', formData)

      // bug
      if (this.isLogin == false) {
        alert("账号或密码错误！请重新输入")
      } else if (this.userToken.length > 0) {
        this.$store.dispatch('getUserInfo')
        this.$router.push('/home')
      }
      
    },

  }
}
</script>

<style>
.userlogin {
  
  text-align: center;
  height: 165px;
  width: 300px;
  background-color: #9fc28d;
  border-radius: 10px;
  padding: 10px 0;
  margin: 30px 0 30px 280px;

}

.userlogin .goBack {
  color: rgb(246, 246, 204);
}

.userlogin ol {
  display: flex;
  flex-flow: column;
  align-items: center;
  color: rgb(250, 255, 215)
}

.userlogin ol li {
  padding: 8px 0;
}

.userlogin ol li i {
  display: inline-block;
  width: 70px;
}

.userlogin input {
  background-color: rgb(246, 246, 204);
  border: 1px solid #8eb57b;
  padding: 3px;
  border-radius: 4px
}

.regButton {
  margin-top: 10px;
}

.regButton button {
  background-color: rgb(246, 246, 204);
  border: 1px solid #8eb57b;
  padding: 3px;
  border-radius: 4px;
  color: #588741;
}

.toLogin {
  display: inline-block;
  margin: 5px 0;
  color: rgb(246, 246, 204);
}
</style>
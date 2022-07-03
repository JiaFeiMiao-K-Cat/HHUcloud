<template>
  <div class="register">
    <router-link to="/home"><div class="goBack">返回</div></router-link>
    <ol>
      <li calss="email"><i>Email:</i><input name="email" type="text" placeholder="请输入邮箱" v-model="email"></li>
      <li class="name"><i>昵称：</i><input name="name" type="text" placeholder="请输入昵称" v-model="username"></li>
      <li class="password"><i>密码：</i><input name="password" type="password" placeholder="请输入密码" v-model="password"></li>
      <li class="password1"><i>确认密码：</i><input name="password1" type="password" placeholder="再次输入密码"
          v-model="password1"></li>
    </ol>
    <div class="regButton">
      <button class="regButtonName" @click="doSubmit()">注&nbsp;&nbsp;册</button>
      <br>
      <router-link to="/login"><span class="toLogin">已有账号?去登录</span></router-link>
    </div>
  </div>

</template>


<script>

// 导入页面
import { mapState } from 'vuex';
export default {
  name: 'register',
  data() {
    return {
      email: '',
      name: '',
      password: '',
      password1: ''
    }
  },
  computed: {
    ...mapState({
      userInfo: (state) => {
        return state.register.searchList
      },
      isRegisted: (state) => {
        return state.register.isRegisted
      }
    })
  },
  components: {

  },
  created() {

  },
  methods: {
    doSubmit() {
      let email = this.email;
      let username = this.username;
      let password = this.password;
      let password1 = this.password1;
      (email && username && password == password1) && this.$store.dispatch('userRegister', { email, username, password })
      if (this.isRegisted == true) {
        alert("该用户已注册！")
      } else {
        this.$router.push('/login')
      }
    }

  }
}
</script>



<style>
.register {
  margin-left: 280px;
  height: 250px;
  width: 300px;
  background-color: #9fc28d;
  border-radius: 10px;
  margin: 30px 0 30px 280px;
  padding:10px;
}

.register .goBack {
  color: rgb(246, 246, 204);
}

.register ol {
  display: flex;
  flex-flow: column;
  color: rgb(250, 255, 215)
}

.register ol li {
  padding: 8px 0;
}

.register ol li i {
  display: inline-block;
  width: 70px;
}

.register input {
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
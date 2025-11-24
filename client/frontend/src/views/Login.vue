<template>
<div class="login-page">

    <div class="login-left"></div>

    <div class="login-right">
        <div class="login-box">
        <h2 class="logo-text">GreenwichURL</h2>
        <h1 class="title">Welcome Back!</h1>

        <form @submit.prevent="Login">
          <div class="ui form">

            <div class="field">
              <label>Username</label>
              <input type="text" v-model="username" placeholder="Enter your username">
            </div>

            <div class="field">
              <label>Password</label>
              <input type="password" v-model="password" placeholder="Enter your password">
            </div>

            <button class="ui button primary login-btn">Login</button>

            <button class="ui button secondary register-btn" @click.prevent="showRegister = true">
              Register
            </button>

            <p class="forgot">
              <a href="#">Forgot password?</a>
            </p>

          </div>
        </form>

      </div>
    </div>

    
    <div v-if="showRegister" class="modal-overlay">
  <div class="modal-content">
    <span class="close-btn" @click="showRegister = false">✖</span>

    <h2 class="register-logo">GreenwichURL</h2>
    <h3 class="register-title">Create Your Account</h3>

    <div class="ui form">

      <div class="field">
        <label>Email</label>
        <input type="email" v-model="regEmail" placeholder="Enter your email">
      </div>

      <div class="field">
        <label>Username</label>
        <input type="text" v-model="regUsername" placeholder="Enter your username">
      </div>

      <div class="field">
        <label>Password</label>
        <input type="password" v-model="regPassword" placeholder="Enter your password">
      </div>

      <div class="field">
        <label>Confirm Password</label>
        <input type="password" v-model="regConfirm" placeholder="Confirm your password">
      </div>

      <button class="ui button primary create-btn" @click.prevent="doRegister">
        Create account
      </button>

      <p class="switch-text">
        <a href="#" @click.prevent="showRegister = false">Back to Login</a>
      </p>

    </div>
  </div>
</div>


</div>
</template>


<script>
import { login, register } from "@/helpers/api";
import "@/styles/login.css"
export default {

  data() {
    return {
      username: "",
      password: "",
      showRegister: false,

      
      regEmail: "",
      regUsername: "",
      regPassword: "",
      regConfirm: ""
    };
  },

  methods: {
    async Login() {
      if (!this.username || !this.password) {
        this.$root.$refs.toast.trigger("Please enter username and password", "error")
        return
      }

      try {
        const res = await login(this.username, this.password)

        localStorage.setItem("token", res.data.token)
        localStorage.setItem("currentUser", JSON.stringify({
          username: this.username,
          role: res.data.role
        }))

        this.$root.$refs.toast.trigger("Login successful!", "success")
        this.$router.push("/home")
      }
      catch {
        this.$root.$refs.toast.trigger("Wrong username or password!", "error")
      }
    },


    
    async doRegister() {
  if (!this.regEmail || !this.regUsername || !this.regPassword || !this.regConfirm) {
    this.$root.$refs.toast.trigger("Please fill all fields", "error")
    return
  }

  if (this.regPassword !== this.regConfirm) {
    this.$root.$refs.toast.trigger("Passwords do not match", "error")
    return
  }

  try {
    await register(this.regUsername, this.regEmail, this.regPassword)

    this.$root.$refs.toast.trigger("Register successful!", "success")

    this.showRegister = false
    this.regEmail = this.regUsername = this.regPassword = this.regConfirm = ""

  } catch (err) {
    this.$root.$refs.toast.trigger(err.response?.data || "Register failed", "error")
  }
}
  }
};
</script>




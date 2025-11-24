<template>
  <div id="app">
    <div class="ui top attached secondary menu navbar">
      
      <div class="item">
        <span class="ui header brand-text">GreenwichURL</span>
      </div>

      <div class="right menu" style="margin-left: auto;">
        <router-link to="/home" class="item nav-item">Home</router-link>
        <router-link to="/myurl" class="item nav-item">MyURL</router-link>

      
        <div class="user-section-btn item">

        
        <div v-if="currentUser" class="user-section-btn item" @click="toggleDropdown">
          <div class="text">{{ currentUser.username }}</div>
          <i class="dropdown icon"></i>

          <div v-if="showDropdown" class="menu">
            <div class="item" @click="logout">Logout</div>
          </div>
        </div>

          
          <router-link v-else to="/login" class="item nav-item">
            Login
          </router-link>
          </div>
        
      </div>
    </div>

    <router-view />
    <Toast ref="toast" />  
  </div>
</template>


<script>
import '@/styles/app.css'
import Toast from "@/components/Toast.vue"
export default {
  components:{Toast},
  data() {
    return {
      currentUser: null,
      showDropdown: false
    };
  },

  mounted() {
    this.loadUser();
  },

  watch: {
    $route() {
      this.loadUser();
      this.showDropdown = false;
    }
  },
  
  methods: {
  loadUser() {
    const saved = localStorage.getItem("currentUser");
    this.currentUser = saved ? JSON.parse(saved) : null;
  },

  logout() {
    localStorage.removeItem("currentUser");
    localStorage.removeItem("token");

    this.currentUser = null;
    this.showDropdown = false;
    this.$router.push("/login");
  },

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }
}

};
</script>


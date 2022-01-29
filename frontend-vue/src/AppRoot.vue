<template>
  <header class="main-header">
    <nav id="nav" class="app-menu">
      <router-link to="/">Algus</router-link> |
      <router-link to="/about">Rakendusest</router-link> |
      <router-link to="/userinteractions">Kasutaja pöördumised</router-link> |
      <a class="external" href="https://localhost:5001/swagger" target="_blank">Swagger</a>
    </nav>
    <div class="global-loader-icon" v-if="this.$store.state.loading" />
  </header>
  <main>
    <router-view />
  </main>
  <aside></aside>
</template>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {

  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}

a.external[target="_blank"]::after {
  content: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAQElEQVR42qXKwQkAIAxDUUdxtO6/RBQkQZvSi8I/pL4BoGw/XPkh4XigPmsUgh0626AjRsgxHTkUThsG2T/sIlzdTsp52kSS1wAAAABJRU5ErkJggg==);
  margin: 0px 3px 0px 5px;
}

.global-loader-icon {
  position: absolute;
  border: 0.5rem solid #f3f3f3;
  border-top: 0.5rem solid #42b983;
  border-radius: 100%;
  width: 1rem;
  height: 1rem;
  animation: spin 0.5s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}
</style>

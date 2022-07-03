const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  transpileDependencies: true,
  
  devServer: {

    proxy: {
      '/api': {
        target: 'https://emocloud.azurewebsites.net',
        ws:true,
        changeOrigin:true,
      }
    }
  }

})



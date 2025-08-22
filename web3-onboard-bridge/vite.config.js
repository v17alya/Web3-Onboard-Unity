import { defineConfig } from 'vite'

// Build a single IIFE bundle for browser injection into Unity WebGL template
export default defineConfig({
  build: {
    lib: {
      entry: './src/index.js',
      name: 'Web3OnboardUnityBundle',
      formats: ['iife'],
      fileName: () => 'web3-onboard-bridge.js'
    },
    sourcemap: false,
    minify: true,
    rollupOptions: {
      output: {
        inlineDynamicImports: true
      }
    }
  },
  define: {
    'process.env': {}
  }
})



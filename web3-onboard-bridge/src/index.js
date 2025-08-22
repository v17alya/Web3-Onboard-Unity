import Onboard from '@web3-onboard/core'
import injectedModule from '@web3-onboard/injected-wallets'
import walletConnectModule from '@web3-onboard/walletconnect'

;(function(){
  /**
   * @typedef {Object} UnityInstance
   * @property {(gameObject: string, methodName: string, argument?: string) => void} SendMessage
   */

  /** Cached Onboard instance */
  let onboardInstance = null
  /** Last connected wallet object from Web3 Onboard */
  let lastWallet = null
  /** Explicitly provided Unity instance (optional) */
  let unityInstance = null
  /** Target GameObject name in Unity scene to receive messages */
  let unityGameObjectName = 'Web3Bridge'

  /**
   * Apply CSS variables theme. Keys should be valid CSS custom properties.
   * @param {Record<string, string>} themeVars
   */
  function applyTheme(themeVars) {
    if (!themeVars || typeof document === 'undefined') return
    const root = document.documentElement
    Object.entries(themeVars).forEach(([key, value]) => {
      try { root.style.setProperty(key, String(value)) } catch {}
    })
  }

  /**
   * Resolve Unity instance by checking provided instance first, then common globals.
   * @returns {UnityInstance | null}
   */
  function getUnity() {
    // Provided instance has priority
    if (unityInstance && typeof unityInstance.SendMessage === 'function') return unityInstance
    // eslint-disable-next-line no-undef
    const w = /** @type {any} */(window)
    const a = w?.MegaMod?.myGameInstance ?? null
    if (a && typeof a.SendMessage === 'function') return a
    const b = w?.unityInstance ?? null
    if (b && typeof b.SendMessage === 'function') return b
    return null
  }

  /**
   * Send a message to Unity (if available)
   * @param {string} method
   * @param {string} [payload]
   */
  function sendToUnity(method, payload) {
    const u = getUnity()
    if (!u) return
    try { u.SendMessage(unityGameObjectName, method, payload ?? '') } catch {}
  }

  /**
   * Initialize Web3-Onboard with project-specific options.
   * If already initialized, it returns the existing instance.
   * @param {Object} [options]
   * @param {string} [options.walletConnectProjectId]
   * @param {Array<{ id: string, token: string, label: string, rpcUrl: string }>} [options.chains]
   * @param {any} [options.appMetadata]
   * @param {any[]} [options.wallets] Optional override for wallet modules
   * @param {{ instance?: UnityInstance, gameObjectName?: string }} [options.unity]
   * @param {Record<string,string>} [options.theme]
   */
  function init(options = {}) {
    if (onboardInstance) return onboardInstance

    // Unity wiring
    if (options.unity?.instance) unityInstance = options.unity.instance
    if (options.unity?.gameObjectName) unityGameObjectName = String(options.unity.gameObjectName)
    // Theme: apply provided or defaults
    if (options.theme) {
      applyTheme(options.theme)
    } else {
      applyTheme({
        '--w3o-background-color': '#1a1d26',
        '--w3o-foreground-color': '#242835',
        '--w3o-text-color': '#eff1fc',
        '--w3o-border-color': '#33394b',
        '--w3o-action-color': '#929bed',
        '--w3o-border-radius': '16px'
      })
    }

    // Wallet modules
    const wallets = Array.isArray(options.wallets) && options.wallets.length > 0
      ? options.wallets
      : (function buildDefaultWallets() {
          const list = []
          // Injected wallets (MetaMask, etc.)
          try { list.push(injectedModule()) } catch {}
          // WalletConnect (optional)
          if (options.walletConnectProjectId) {
            try { list.push(walletConnectModule({ projectId: options.walletConnectProjectId })) } catch {}
          }
          return list
        })()

    // Chains
    const chains = Array.isArray(options.chains) && options.chains.length > 0
      ? options.chains
      : [
          {
            id: '0x2105',
            token: 'ETH',
            label: 'Base',
            rpcUrl: 'https://mainnet.base.org'
          }
        ]

    // App metadata
    const appMetadata = options.appMetadata || {
      name: 'Unity Web3 Onboard',
      description: 'Web3-Onboard bridge for Unity WebGL',
      icon: '<svg></svg>',
      recommendedInjectedWallets: [
        { name: 'MetaMask', url: 'https://metamask.io' }
      ]
    }

    onboardInstance = Onboard({ wallets, chains, appMetadata })
    return onboardInstance
  }

  function requireOnboard() {
    if (!onboardInstance) {
      throw new Error('OnboardWalletBridge is not initialized. Call OnboardWalletBridge.init(...) first.')
    }
    return onboardInstance
  }

  /**
   * Connect via Web3-Onboard and notify Unity
   * @param {Object} [options]
   */
  async function connect() {
    const onboard = requireOnboard()
    try {
      const wallets = await onboard.connectWallet()
      lastWallet = wallets?.[0] || null
      const address = lastWallet?.accounts?.[0]?.address || ''
      if (!address) throw new Error('No address from wallet')
      sendToUnity('OnConnected', address)
      return wallets
    } catch (e) {
      const msg = /** @type {any} */(e)?.message ?? 'Connect error'
      sendToUnity('OnConnectError', String(msg))
      throw e
    }
  }

  /**
   * Disconnect all connected wallets and notify Unity
   */
  async function disconnect() {
    const onboard = requireOnboard()
    try {
      const state = onboard.state.get()
      for (const w of state.wallets) {
        try { await onboard.disconnectWallet({ label: w.label }) } catch {}
      }
      lastWallet = null
      sendToUnity('OnDisconnected', '')
    } catch (e) {
      const msg = /** @type {any} */(e)?.message ?? 'Disconnect error'
      sendToUnity('OnDisconnectError', String(msg))
      throw e
    }
  }

  /**
   * Sign an arbitrary message with the last connected wallet
   * @param {string} message
   */
  async function signMessage(message) {
    try {
      if (!lastWallet) throw new Error('No wallet connected')
      const addr = lastWallet.accounts?.[0]?.address
      const sig = await lastWallet.provider.request({
        method: 'personal_sign',
        params: [message, addr]
      })
      sendToUnity('OnSigned', String(sig))
      return sig
    } catch (e) {
      const msg = /** @type {any} */(e)?.message ?? 'Sign error'
      sendToUnity('OnSignError', String(msg))
      throw e
    }
  }

  /**
   * Set Unity instance explicitly.
   * @param {UnityInstance} instance
   */
  function setUnityInstance(instance) {
    unityInstance = instance || null
  }

  // Expose API on a single global: OnboardWalletBridge
  window.OnboardWalletBridge = {
    init,
    connect,
    disconnect,
    signMessage,
    setUnityInstance,
    getUnity,
    sendToUnity,
    applyTheme
  }
})();



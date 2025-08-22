mergeInto(LibraryManager.library, {
  W3O_Init: function (optionsPtr) {
    try {
      var optionsJson = UTF8ToString(optionsPtr);
      var options = null;
      if (optionsJson && optionsJson.length) {
        try { options = JSON.parse(optionsJson); } catch (e) { console.error('[W3O_Init] Failed to parse options JSON:', e); }
      }
      if (typeof window === 'undefined' || !window.OnboardWalletBridge || !window.OnboardWalletBridge.init) {
        console.error('[W3O_Init] OnboardWalletBridge.init not found');
        return 0;
      }
      window.OnboardWalletBridge.init(options || {});
      return 1;
    } catch (e) {
      console.error('[W3O_Init] Error:', e);
      return 0;
    }
  },

  W3O_Connect: function () {
    try {
      if (window && window.OnboardWalletBridge && window.OnboardWalletBridge.connect) {
        window.OnboardWalletBridge.connect();
      } else {
        console.error('[W3O_Connect] OnboardWalletBridge.connect not found');
      }
    } catch (e) { console.error('[W3O_Connect] Error:', e); }
  },

  W3O_DisconnectAll: function () {
    try {
      if (window && window.OnboardWalletBridge && window.OnboardWalletBridge.disconnect) {
        window.OnboardWalletBridge.disconnect();
      } else {
        console.error('[W3O_DisconnectAll] OnboardWalletBridge.disconnect not found');
      }
    } catch (e) { console.error('[W3O_DisconnectAll] Error:', e); }
  },

  W3O_ApplyTheme: function (themePtr) {
    try {
      var themeJson = UTF8ToString(themePtr);
      var theme = null;
      if (themeJson && themeJson.length) {
        try { theme = JSON.parse(themeJson); } catch (e) { console.error('[W3O_ApplyTheme] Failed to parse theme JSON:', e); }
      }
      if (window && window.OnboardWalletBridge && window.OnboardWalletBridge.applyTheme) {
        window.OnboardWalletBridge.applyTheme(theme || {});
      } else {
        console.error('[W3O_ApplyTheme] OnboardWalletBridge.applyTheme not found');
      }
    } catch (e) { console.error('[W3O_ApplyTheme] Error:', e); }
  },

  W3O_SignMessage: function (messagePtr, goPtr, methodPtr) {
    var message = UTF8ToString(messagePtr);
    var goName = UTF8ToString(goPtr);
    var method = UTF8ToString(methodPtr);
    try {
      if (!(window && window.OnboardWalletBridge && window.OnboardWalletBridge.signMessage)) {
        console.error('[W3O_SignMessage] OnboardWalletBridge.signMessage not found');
        if (typeof SendMessage === 'function') {
          SendMessage(goName, method, JSON.stringify({ ok: false, error: 'signMessage missing' }));
        }
        return;
      }
      var result = window.OnboardWalletBridge.signMessage(message || '');
      if (result && typeof result.then === 'function') {
        result.then(function (val) {
          if (typeof SendMessage === 'function') {
            SendMessage(goName, method, JSON.stringify({ ok: true, result: val }));
          }
        }).catch(function (err) {
          if (typeof SendMessage === 'function') {
            var msg = (err && err.message) ? err.message : String(err);
            SendMessage(goName, method, JSON.stringify({ ok: false, error: msg }));
          }
        });
      } else {
        if (typeof SendMessage === 'function') {
          SendMessage(goName, method, JSON.stringify({ ok: true, result: result }));
        }
      }
    } catch (e) {
      console.error('[W3O_SignMessage] Error:', e);
      if (typeof SendMessage === 'function') {
        var msg = (e && e.message) ? e.message : String(e);
        SendMessage(goName, method, JSON.stringify({ ok: false, error: msg }));
      }
    }
  },

  W3O_SetUnityInstance: function (goPtr) {
    try {
      var goName = UTF8ToString(goPtr);
      if (window && window.OnboardWalletBridge && window.OnboardWalletBridge.setUnityInstance) {
        window.OnboardWalletBridge.setUnityInstance({ gameObjectName: goName });
      } else {
        console.error('[W3O_SetUnityInstance] OnboardWalletBridge.setUnityInstance not found');
      }
    } catch (e) { console.error('[W3O_SetUnityInstance] Error:', e); }
  },

  W3O_SendToUnity: function (goPtr, methodPtr, payloadPtr) {
    try {
      var goName = UTF8ToString(goPtr);
      var method = UTF8ToString(methodPtr);
      var payload = UTF8ToString(payloadPtr);
      if (typeof SendMessage === 'function') {
        SendMessage(goName, method, payload || '');
      } else {
        console.error('[W3O_SendToUnity] SendMessage not available');
      }
    } catch (e) { console.error('[W3O_SendToUnity] Error:', e); }
  }
});



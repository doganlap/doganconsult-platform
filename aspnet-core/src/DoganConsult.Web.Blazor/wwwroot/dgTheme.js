window.dgTheme = {
  apply: (config) => {
    // Apply CSS variables
    document.documentElement.style.setProperty('--dg-primary', config.primary || '#0ea5a4');
    document.documentElement.style.setProperty('--dg-accent', config.accent || '#22c55e');
    
    // Apply app title
    if (config.appName) {
      document.title = config.appName + ' - DG.OS';
    }
    
    // Apply RTL
    document.documentElement.dir = config.rtl ? 'rtl' : 'ltr';
    document.documentElement.lang = 'en';
    
    // Apply logo if provided
    if (config.logoUrl) {
      const logoElements = document.querySelectorAll('[data-dg-logo]');
      logoElements.forEach(el => {
        el.src = config.logoUrl;
      });
    }
    
    // Apply favicon if provided
    if (config.faviconUrl) {
      let favicon = document.querySelector("link[rel='icon']");
      if (!favicon) {
        favicon = document.createElement('link');
        favicon.rel = 'icon';
        document.head.appendChild(favicon);
      }
      favicon.href = config.faviconUrl;
    }
    
    console.log('DG.OS Theme applied:', config);
  }
};

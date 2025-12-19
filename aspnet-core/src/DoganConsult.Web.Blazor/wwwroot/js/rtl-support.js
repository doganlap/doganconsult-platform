// RTL (Right-to-Left) Dynamic Support
// Automatically updates document direction when language changes

window.updateDocumentDirection = function (direction) {
    document.documentElement.setAttribute('dir', direction);
    document.documentElement.setAttribute('lang', direction === 'rtl' ? 'ar' : 'en');
    
    // Dispatch event for components that need to react to RTL changes
    window.dispatchEvent(new CustomEvent('rtlChanged', { detail: { direction } }));
    
    console.log(`Document direction updated to: ${direction}`);
};

window.isRtl = function () {
    return document.documentElement.getAttribute('dir') === 'rtl';
};

// Initialize on load
document.addEventListener('DOMContentLoaded', function () {
    const currentDir = document.documentElement.getAttribute('dir') || 'ltr';
    console.log(`RTL Support initialized. Current direction: ${currentDir}`);
});


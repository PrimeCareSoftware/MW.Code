/**
 * Responsive dialog and component sizing constants
 * Ensures consistent responsive behavior across the application
 */

export const ResponsiveDialog = {
  /**
   * Small dialog (forms, confirmations)
   */
  SMALL: {
    width: 'min(400px, 95vw)',
    maxWidth: '95vw'
  },
  
  /**
   * Medium dialog (standard content)
   */
  MEDIUM: {
    width: 'min(600px, 95vw)',
    maxWidth: '95vw'
  },
  
  /**
   * Large dialog (extensive content)
   */
  LARGE: {
    width: 'min(800px, 95vw)',
    maxWidth: '95vw'
  },
  
  /**
   * Extra large dialog (extensive content)
   */
  XLARGE: {
    width: 'min(1000px, 95vw)',
    maxWidth: '95vw'
  },
  
  /**
   * Full width dialog on mobile
   */
  FULL_MOBILE: {
    width: 'min(600px, 100vw)',
    maxWidth: '100vw'
  }
};

/**
 * Breakpoints matching Material Design
 */
export const Breakpoints = {
  XS: 0,      // Extra small devices (phones)
  SM: 600,    // Small devices (tablets portrait)
  MD: 960,    // Medium devices (tablets landscape)
  LG: 1280,   // Large devices (desktops)
  XL: 1920    // Extra large devices (large desktops)
};

/**
 * Check if current viewport matches a breakpoint
 */
export function isBreakpoint(breakpoint: keyof typeof Breakpoints): boolean {
  return window.innerWidth >= Breakpoints[breakpoint];
}

/**
 * Get appropriate dialog config based on viewport
 */
export function getResponsiveDialogConfig(size: 'small' | 'medium' | 'large' = 'medium') {
  const sizeMap = {
    small: ResponsiveDialog.SMALL,
    medium: ResponsiveDialog.MEDIUM,
    large: ResponsiveDialog.LARGE
  };
  
  return sizeMap[size];
}

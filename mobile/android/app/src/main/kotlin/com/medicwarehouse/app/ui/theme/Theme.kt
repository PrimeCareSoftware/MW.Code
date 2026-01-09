package com.medicwarehouse.app.ui.theme

import android.app.Activity
import android.os.Build
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.dynamicDarkColorScheme
import androidx.compose.material3.dynamicLightColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.SideEffect
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.toArgb
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalView
import androidx.core.view.WindowCompat

// Primary Brand Colors
val MWPrimary = Color(0xFF4F46E5)           // Indigo 600
val MWPrimaryLight = Color(0xFF818CF8)      // Indigo 400
val MWPrimaryDark = Color(0xFF3730A3)       // Indigo 800
val MWPrimaryContainer = Color(0xFFE0E7FF)  // Indigo 100

// Secondary Colors
val MWSecondary = Color(0xFF8B5CF6)         // Violet 500
val MWSecondaryLight = Color(0xFFA78BFA)    // Violet 400
val MWSecondaryContainer = Color(0xFFEDE9FE) // Violet 100

// Tertiary / Accent Colors
val MWTertiary = Color(0xFF06B6D4)          // Cyan 500
val MWTertiaryLight = Color(0xFF22D3EE)     // Cyan 400
val MWTertiaryContainer = Color(0xFFCFFAFE) // Cyan 100

// Semantic Colors
val MWSuccess = Color(0xFF10B981)           // Emerald 500
val MWSuccessLight = Color(0xFF34D399)      // Emerald 400
val MWSuccessContainer = Color(0xFFD1FAE5)  // Emerald 100

val MWWarning = Color(0xFFF59E0B)           // Amber 500
val MWWarningLight = Color(0xFFFBBF24)      // Amber 400
val MWWarningContainer = Color(0xFFFEF3C7)  // Amber 100

val MWError = Color(0xFFEF4444)             // Red 500
val MWErrorLight = Color(0xFFF87171)        // Red 400
val MWErrorContainer = Color(0xFFFEE2E2)    // Red 100

val MWInfo = Color(0xFF3B82F6)              // Blue 500
val MWInfoContainer = Color(0xFFDBEAFE)     // Blue 100

// Neutral Colors - Light
val MWBackground = Color(0xFFF8FAFC)        // Slate 50
val MWSurface = Color(0xFFFFFFFF)           // White
val MWSurfaceSecondary = Color(0xFFF1F5F9)  // Slate 100
val MWSurfaceTertiary = Color(0xFFE2E8F0)   // Slate 200
val MWBorder = Color(0xFFE2E8F0)            // Slate 200
val MWTextPrimary = Color(0xFF0F172A)       // Slate 900
val MWTextSecondary = Color(0xFF64748B)     // Slate 500
val MWTextMuted = Color(0xFF94A3B8)         // Slate 400

// Neutral Colors - Dark
val MWBackgroundDark = Color(0xFF0F172A)    // Slate 900
val MWSurfaceDark = Color(0xFF1E293B)       // Slate 800
val MWSurfaceSecondaryDark = Color(0xFF334155) // Slate 700
val MWBorderDark = Color(0xFF475569)        // Slate 600
val MWTextPrimaryDark = Color(0xFFF8FAFC)   // Slate 50
val MWTextSecondaryDark = Color(0xFFCBD5E1) // Slate 300
val MWTextMutedDark = Color(0xFF94A3B8)     // Slate 400

private val DarkColorScheme = darkColorScheme(
    primary = MWPrimary,
    onPrimary = Color.White,
    primaryContainer = MWPrimaryDark,
    onPrimaryContainer = MWPrimaryLight,
    secondary = MWSecondary,
    onSecondary = Color.White,
    secondaryContainer = Color(0xFF4C1D95),
    onSecondaryContainer = MWSecondaryLight,
    tertiary = MWTertiary,
    onTertiary = Color.White,
    tertiaryContainer = Color(0xFF164E63),
    onTertiaryContainer = MWTertiaryLight,
    background = MWBackgroundDark,
    onBackground = MWTextPrimaryDark,
    surface = MWSurfaceDark,
    onSurface = MWTextPrimaryDark,
    surfaceVariant = MWSurfaceSecondaryDark,
    onSurfaceVariant = MWTextSecondaryDark,
    outline = MWBorderDark,
    outlineVariant = Color(0xFF334155),
    error = MWError,
    onError = Color.White,
    errorContainer = Color(0xFF7F1D1D),
    onErrorContainer = MWErrorLight
)

private val LightColorScheme = lightColorScheme(
    primary = MWPrimary,
    onPrimary = Color.White,
    primaryContainer = MWPrimaryContainer,
    onPrimaryContainer = MWPrimaryDark,
    secondary = MWSecondary,
    onSecondary = Color.White,
    secondaryContainer = MWSecondaryContainer,
    onSecondaryContainer = Color(0xFF5B21B6),
    tertiary = MWTertiary,
    onTertiary = Color.White,
    tertiaryContainer = MWTertiaryContainer,
    onTertiaryContainer = Color(0xFF155E75),
    background = MWBackground,
    onBackground = MWTextPrimary,
    surface = MWSurface,
    onSurface = MWTextPrimary,
    surfaceVariant = MWSurfaceSecondary,
    onSurfaceVariant = MWTextSecondary,
    outline = MWBorder,
    outlineVariant = MWSurfaceTertiary,
    error = MWError,
    onError = Color.White,
    errorContainer = MWErrorContainer,
    onErrorContainer = Color(0xFFB91C1C)
)

@Composable
fun PrimeCare SoftwareTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    dynamicColor: Boolean = false,
    content: @Composable () -> Unit
) {
    val colorScheme = when {
        dynamicColor && Build.VERSION.SDK_INT >= Build.VERSION_CODES.S -> {
            val context = LocalContext.current
            if (darkTheme) dynamicDarkColorScheme(context) else dynamicLightColorScheme(context)
        }
        darkTheme -> DarkColorScheme
        else -> LightColorScheme
    }
    
    val view = LocalView.current
    if (!view.isInEditMode) {
        SideEffect {
            val window = (view.context as Activity).window
            window.statusBarColor = colorScheme.background.toArgb()
            WindowCompat.getInsetsController(window, view).isAppearanceLightStatusBars = !darkTheme
        }
    }
    
    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography,
        shapes = Shapes,
        content = content
    )
}

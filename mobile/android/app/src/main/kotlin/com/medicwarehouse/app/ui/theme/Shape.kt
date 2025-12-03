package com.medicwarehouse.app.ui.theme

import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Shapes
import androidx.compose.ui.unit.dp

val Shapes = Shapes(
    extraSmall = RoundedCornerShape(4.dp),
    small = RoundedCornerShape(8.dp),
    medium = RoundedCornerShape(12.dp),
    large = RoundedCornerShape(16.dp),
    extraLarge = RoundedCornerShape(20.dp)
)

// Custom shape constants for consistent UI
object MWShapes {
    val CardRadius = 16.dp
    val ButtonRadius = 12.dp
    val ChipRadius = 20.dp
    val InputRadius = 12.dp
    val AvatarRadius = 100.dp
    val BottomSheetRadius = 28.dp
}

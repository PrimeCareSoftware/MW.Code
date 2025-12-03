package com.medicwarehouse.app.ui.screens

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.core.*
import androidx.compose.animation.fadeIn
import androidx.compose.animation.slideInVertically
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material.icons.outlined.Business
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.medicwarehouse.app.ui.components.MWPrimaryButton
import com.medicwarehouse.app.ui.components.MWTextField
import com.medicwarehouse.app.ui.theme.*
import com.medicwarehouse.app.viewmodel.AuthViewModel
import kotlinx.coroutines.delay

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginScreen(
    onLoginSuccess: () -> Unit,
    viewModel: AuthViewModel = hiltViewModel()
) {
    var username by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var tenantId by remember { mutableStateOf("") }
    var isOwnerLogin by remember { mutableStateOf(false) }
    
    val isLoading by viewModel.isLoading.collectAsState()
    val errorMessage by viewModel.errorMessage.collectAsState()
    val isAuthenticated by viewModel.isAuthenticated.collectAsState()
    
    // Animation states
    var showLogo by remember { mutableStateOf(false) }
    var showForm by remember { mutableStateOf(false) }
    val logoScale by animateFloatAsState(
        targetValue = if (showLogo) 1f else 0.5f,
        animationSpec = spring(dampingRatio = 0.6f, stiffness = 300f),
        label = "logo_scale"
    )
    
    // Animated background
    val infiniteTransition = rememberInfiniteTransition(label = "background")
    val animatedOffset by infiniteTransition.animateFloat(
        initialValue = 0f,
        targetValue = 1f,
        animationSpec = infiniteRepeatable(
            animation = tween(5000, easing = LinearEasing),
            repeatMode = RepeatMode.Reverse
        ),
        label = "gradient_animation"
    )
    
    LaunchedEffect(isAuthenticated) {
        if (isAuthenticated) {
            onLoginSuccess()
        }
    }
    
    LaunchedEffect(Unit) {
        delay(100)
        showLogo = true
        delay(300)
        showForm = true
    }
    
    val isFormValid = username.isNotBlank() && password.isNotBlank() && tenantId.isNotBlank()
    
    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(
                brush = Brush.linearGradient(
                    colors = listOf(
                        MWPrimary,
                        MWSecondary.copy(alpha = 0.9f + animatedOffset * 0.1f),
                        Color(0xFFEC4899).copy(alpha = 0.7f)
                    )
                )
            )
    ) {
        // Decorative blurred circles
        Box(
            modifier = Modifier
                .size(300.dp)
                .offset(x = (-100).dp, y = (-100).dp)
                .blur(60.dp)
                .clip(CircleShape)
                .background(Color.White.copy(alpha = 0.1f))
        )
        Box(
            modifier = Modifier
                .size(250.dp)
                .align(Alignment.BottomEnd)
                .offset(x = 50.dp, y = 100.dp)
                .blur(50.dp)
                .clip(CircleShape)
                .background(MWTertiary.copy(alpha = 0.15f))
        )
        Box(
            modifier = Modifier
                .size(200.dp)
                .align(Alignment.CenterEnd)
                .offset(x = 80.dp, y = (-50).dp)
                .blur(40.dp)
                .clip(CircleShape)
                .background(MWSecondary.copy(alpha = 0.1f))
        )
        
        Column(
            modifier = Modifier
                .fillMaxSize()
                .verticalScroll(rememberScrollState())
                .padding(24.dp)
                .statusBarsPadding()
                .navigationBarsPadding(),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Spacer(modifier = Modifier.height(48.dp))
            
            // Logo and Title
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier.scale(logoScale)
            ) {
                // Logo with glow effect
                Box(contentAlignment = Alignment.Center) {
                    // Glow
                    Box(
                        modifier = Modifier
                            .size(120.dp)
                            .blur(20.dp)
                            .clip(CircleShape)
                            .background(Color.White.copy(alpha = 0.2f))
                    )
                    // Logo container
                    Surface(
                        modifier = Modifier.size(100.dp),
                        shape = CircleShape,
                        color = Color.White.copy(alpha = 0.15f),
                        tonalElevation = 0.dp,
                        shadowElevation = 16.dp
                    ) {
                        Box(contentAlignment = Alignment.Center) {
                            Icon(
                                imageVector = Icons.Default.MedicalServices,
                                contentDescription = "Logo",
                                modifier = Modifier.size(48.dp),
                                tint = Color.White
                            )
                        }
                    }
                }
                
                Spacer(modifier = Modifier.height(20.dp))
                
                Text(
                    text = "MedicWarehouse",
                    style = MaterialTheme.typography.headlineLarge.copy(
                        fontSize = 32.sp,
                        fontWeight = FontWeight.Bold
                    ),
                    color = Color.White
                )
                
                Text(
                    text = "Sistema de Gestão Médica",
                    style = MaterialTheme.typography.bodyMedium,
                    color = Color.White.copy(alpha = 0.85f)
                )
            }
            
            Spacer(modifier = Modifier.height(40.dp))
            
            // Login Form Card
            AnimatedVisibility(
                visible = showForm,
                enter = fadeIn() + slideInVertically { it / 2 }
            ) {
                Surface(
                    modifier = Modifier.fillMaxWidth(),
                    shape = RoundedCornerShape(28.dp),
                    color = Color.White.copy(alpha = 0.12f),
                    tonalElevation = 0.dp
                ) {
                    Column(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(24.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        // Username Field
                        MWTextField(
                            value = username,
                            onValueChange = { username = it },
                            label = "Usuário",
                            leadingIcon = Icons.Default.Person,
                            imeAction = ImeAction.Next
                        )
                        
                        // Password Field
                        MWTextField(
                            value = password,
                            onValueChange = { password = it },
                            label = "Senha",
                            leadingIcon = Icons.Default.Lock,
                            isPassword = true,
                            imeAction = ImeAction.Next
                        )
                        
                        // Tenant ID Field
                        MWTextField(
                            value = tenantId,
                            onValueChange = { tenantId = it },
                            label = "ID da Clínica",
                            leadingIcon = Icons.Outlined.Business,
                            imeAction = ImeAction.Done
                        )
                        
                        // Owner Login Toggle
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Switch(
                                checked = isOwnerLogin,
                                onCheckedChange = { isOwnerLogin = it },
                                colors = SwitchDefaults.colors(
                                    checkedThumbColor = Color.White,
                                    checkedTrackColor = MWTertiary,
                                    uncheckedThumbColor = Color.White,
                                    uncheckedTrackColor = Color.White.copy(alpha = 0.3f)
                                )
                            )
                            Spacer(modifier = Modifier.width(12.dp))
                            Text(
                                text = "Login como proprietário",
                                style = MaterialTheme.typography.bodyMedium,
                                color = Color.White
                            )
                        }
                        
                        // Error Message
                        AnimatedVisibility(visible = errorMessage != null) {
                            Surface(
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp),
                                color = MWError.copy(alpha = 0.15f)
                            ) {
                                Row(
                                    modifier = Modifier.padding(12.dp),
                                    verticalAlignment = Alignment.CenterVertically,
                                    horizontalArrangement = Arrangement.spacedBy(8.dp)
                                ) {
                                    Icon(
                                        imageVector = Icons.Default.Warning,
                                        contentDescription = null,
                                        modifier = Modifier.size(18.dp),
                                        tint = MWError
                                    )
                                    Text(
                                        text = errorMessage ?: "",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MWError
                                    )
                                }
                            }
                        }
                        
                        Spacer(modifier = Modifier.height(8.dp))
                        
                        // Login Button
                        MWPrimaryButton(
                            text = "Entrar",
                            onClick = {
                                viewModel.login(username, password, tenantId, isOwnerLogin)
                            },
                            enabled = isFormValid,
                            isLoading = isLoading,
                            icon = Icons.Default.ArrowForward
                        )
                    }
                }
            }
            
            Spacer(modifier = Modifier.weight(1f))
            
            // Footer
            AnimatedVisibility(visible = showForm) {
                Text(
                    text = "Versão 1.0.0",
                    style = MaterialTheme.typography.labelSmall,
                    color = Color.White.copy(alpha = 0.6f)
                )
            }
            
            Spacer(modifier = Modifier.height(16.dp))
        }
    }
}

package com.medicwarehouse.app.ui.screens

import androidx.compose.animation.*
import androidx.compose.animation.core.*
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material.icons.outlined.Notifications
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.medicwarehouse.app.ui.components.*
import com.medicwarehouse.app.ui.theme.*
import com.medicwarehouse.app.viewmodel.AuthViewModel
import com.medicwarehouse.app.viewmodel.DashboardViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.*

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardScreen(
    onLogout: () -> Unit,
    authViewModel: AuthViewModel = hiltViewModel(),
    dashboardViewModel: DashboardViewModel = hiltViewModel()
) {
    val stats by dashboardViewModel.stats.collectAsState()
    val isLoading by dashboardViewModel.isLoading.collectAsState()
    val currentUser by authViewModel.currentUser.collectAsState()
    
    val scope = rememberCoroutineScope()
    val snackbarHostState = remember { SnackbarHostState() }
    
    // Get greeting based on time of day
    val greeting = remember {
        val hour = Calendar.getInstance().get(Calendar.HOUR_OF_DAY)
        when {
            hour < 12 -> "Bom dia!"
            hour < 18 -> "Boa tarde!"
            else -> "Boa noite!"
        }
    }
    
    // Get formatted date
    val dateString = remember {
        val dateFormat = SimpleDateFormat("EEEE, d 'de' MMMM", Locale("pt", "BR"))
        dateFormat.format(Date()).replaceFirstChar { it.uppercase() }
    }
    
    // Animation states
    var showContent by remember { mutableStateOf(false) }
    
    LaunchedEffect(Unit) {
        delay(100)
        showContent = true
    }
    
    Scaffold(
        snackbarHost = { SnackbarHost(snackbarHostState) },
        containerColor = MaterialTheme.colorScheme.background
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues),
            contentPadding = PaddingValues(
                start = 20.dp,
                end = 20.dp,
                top = 16.dp,
                bottom = 24.dp
            ),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            // Header Section
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn() + slideInVertically { -it / 2 }
                ) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.Top
                    ) {
                        Column {
                            Text(
                                text = greeting,
                                style = MaterialTheme.typography.headlineMedium.copy(
                                    fontSize = 28.sp,
                                    fontWeight = FontWeight.Bold
                                ),
                                color = MaterialTheme.colorScheme.onBackground
                            )
                            Text(
                                text = dateString,
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                        }
                        
                        // Notification button
                        Surface(
                            modifier = Modifier.size(48.dp),
                            shape = CircleShape,
                            color = MaterialTheme.colorScheme.surfaceVariant,
                            onClick = {
                                scope.launch {
                                    snackbarHostState.showSnackbar("Notificações em desenvolvimento")
                                }
                            }
                        ) {
                            Box(contentAlignment = Alignment.Center) {
                                Icon(
                                    imageVector = Icons.Outlined.Notifications,
                                    contentDescription = "Notificações",
                                    modifier = Modifier.size(22.dp),
                                    tint = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                                // Notification badge
                                Box(
                                    modifier = Modifier
                                        .align(Alignment.TopEnd)
                                        .offset(x = (-8).dp, y = 8.dp)
                                        .size(10.dp)
                                        .clip(CircleShape)
                                        .background(MWError)
                                )
                            }
                        }
                    }
                }
            }
            
            // Section Title - Stats
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 100))
                ) {
                    Text(
                        text = "Resumo do Dia",
                        style = MaterialTheme.typography.titleSmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                        modifier = Modifier.padding(top = 8.dp)
                    )
                }
            }
            
            // Stats Cards
            item {
                if (isLoading) {
                    // Skeleton loading
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        repeat(2) {
                            MWCard(modifier = Modifier.weight(1f)) {
                                Column(
                                    modifier = Modifier.padding(16.dp),
                                    verticalArrangement = Arrangement.spacedBy(12.dp)
                                ) {
                                    MWSkeletonCircle(size = 42.dp)
                                    Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                                        MWSkeletonBox(modifier = Modifier.width(60.dp), height = 28.dp)
                                        MWSkeletonBox(modifier = Modifier.width(80.dp), height = 14.dp)
                                    }
                                }
                            }
                        }
                    }
                } else {
                    AnimatedVisibility(
                        visible = showContent,
                        enter = fadeIn(animationSpec = tween(delayMillis = 150)) +
                                slideInVertically(initialOffsetY = { it / 4 })
                    ) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(12.dp)
                        ) {
                            MWStatCard(
                                title = "Consultas Hoje",
                                value = stats?.todayAppointments?.toString() ?: "0",
                                icon = Icons.Default.CalendarToday,
                                gradientColors = listOf(MWPrimary, MWPrimaryLight),
                                modifier = Modifier.weight(1f)
                            )
                            MWStatCard(
                                title = "Total Pacientes",
                                value = stats?.totalPatients?.toString() ?: "0",
                                icon = Icons.Default.People,
                                gradientColors = listOf(MWSuccess, MWSuccessLight),
                                modifier = Modifier.weight(1f)
                            )
                        }
                    }
                }
            }
            
            item {
                if (isLoading) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        repeat(2) {
                            MWCard(modifier = Modifier.weight(1f)) {
                                Column(
                                    modifier = Modifier.padding(16.dp),
                                    verticalArrangement = Arrangement.spacedBy(12.dp)
                                ) {
                                    MWSkeletonCircle(size = 42.dp)
                                    Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                                        MWSkeletonBox(modifier = Modifier.width(60.dp), height = 28.dp)
                                        MWSkeletonBox(modifier = Modifier.width(80.dp), height = 14.dp)
                                    }
                                }
                            }
                        }
                    }
                } else {
                    AnimatedVisibility(
                        visible = showContent,
                        enter = fadeIn(animationSpec = tween(delayMillis = 200)) +
                                slideInVertically(initialOffsetY = { it / 4 })
                    ) {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.spacedBy(12.dp)
                        ) {
                            MWStatCard(
                                title = "Pendentes",
                                value = stats?.pendingAppointments?.toString() ?: "0",
                                icon = Icons.Default.Schedule,
                                gradientColors = listOf(MWWarning, MWWarningLight),
                                modifier = Modifier.weight(1f)
                            )
                            MWStatCard(
                                title = "Concluídas",
                                value = stats?.completedToday?.toString() ?: "0",
                                icon = Icons.Default.CheckCircle,
                                gradientColors = listOf(MWSecondary, MWSecondaryLight),
                                modifier = Modifier.weight(1f)
                            )
                        }
                    }
                }
            }
            
            // Section Title - Quick Actions
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 250))
                ) {
                    Text(
                        text = "Ações Rápidas",
                        style = MaterialTheme.typography.titleSmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                        modifier = Modifier.padding(top = 8.dp)
                    )
                }
            }
            
            // Quick Action Cards
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 300)) +
                            slideInHorizontally(initialOffsetX = { -it / 4 })
                ) {
                    MWQuickActionCard(
                        title = "Novo Paciente",
                        subtitle = "Cadastrar paciente",
                        icon = Icons.Default.PersonAdd,
                        gradientColors = listOf(MWPrimary, MWPrimaryLight),
                        onClick = {
                            scope.launch {
                                snackbarHostState.showSnackbar("Funcionalidade em desenvolvimento")
                            }
                        }
                    )
                }
            }
            
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 350)) +
                            slideInHorizontally(initialOffsetX = { -it / 4 })
                ) {
                    MWQuickActionCard(
                        title = "Novo Agendamento",
                        subtitle = "Agendar consulta",
                        icon = Icons.Default.EventAvailable,
                        gradientColors = listOf(MWSuccess, MWSuccessLight),
                        onClick = {
                            scope.launch {
                                snackbarHostState.showSnackbar("Funcionalidade em desenvolvimento")
                            }
                        }
                    )
                }
            }
            
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 400)) +
                            slideInHorizontally(initialOffsetX = { -it / 4 })
                ) {
                    MWQuickActionCard(
                        title = "Ver Agenda Completa",
                        subtitle = "Visualizar todos os agendamentos",
                        icon = Icons.Default.CalendarMonth,
                        gradientColors = listOf(MWWarning, MWWarningLight),
                        onClick = {
                            scope.launch {
                                snackbarHostState.showSnackbar("Funcionalidade em desenvolvimento")
                            }
                        }
                    )
                }
            }
            
            // Profile Section
            item {
                AnimatedVisibility(
                    visible = showContent,
                    enter = fadeIn(animationSpec = tween(delayMillis = 450))
                ) {
                    Spacer(modifier = Modifier.height(8.dp))
                    
                    MWCard(
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(16.dp),
                            verticalAlignment = Alignment.CenterVertically,
                            horizontalArrangement = Arrangement.spacedBy(16.dp)
                        ) {
                            MWAvatar(
                                name = currentUser?.username ?: "U",
                                size = 52.dp
                            )
                            
                            Column(modifier = Modifier.weight(1f)) {
                                Text(
                                    text = currentUser?.username ?: "Usuário",
                                    style = MaterialTheme.typography.titleMedium,
                                    fontWeight = FontWeight.SemiBold,
                                    color = MaterialTheme.colorScheme.onSurface
                                )
                                Row(
                                    horizontalArrangement = Arrangement.spacedBy(8.dp),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    currentUser?.let { user ->
                                        MWStatusBadge(
                                            text = user.role,
                                            color = MWPrimary,
                                            style = BadgeStyle.Subtle
                                        )
                                    }
                                }
                                Text(
                                    text = currentUser?.tenantId ?: "",
                                    style = MaterialTheme.typography.bodySmall,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                            
                            // Logout button
                            IconButton(
                                onClick = {
                                    authViewModel.logout()
                                    onLogout()
                                }
                            ) {
                                Icon(
                                    imageVector = Icons.Default.Logout,
                                    contentDescription = "Sair",
                                    tint = MWError
                                )
                            }
                        }
                    }
                }
            }
        }
    }
}

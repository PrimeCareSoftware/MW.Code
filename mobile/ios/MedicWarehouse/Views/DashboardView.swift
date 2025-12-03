import SwiftUI

struct DashboardView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    @State private var stats: DashboardStats?
    @State private var isLoading = true
    @State private var selectedTab = 0
    
    private let apiService = APIService()
    
    var body: some View {
        TabView(selection: $selectedTab) {
            // Home Tab
            HomeTabView(stats: stats, isLoading: isLoading, refreshAction: {
                await loadDashboardStats()
            })
            .tabItem {
                TabItemView(icon: "house.fill", title: "Início", isSelected: selectedTab == 0)
            }
            .tag(0)
            
            // Patients Tab
            PatientsView()
                .tabItem {
                    TabItemView(icon: "person.3.fill", title: "Pacientes", isSelected: selectedTab == 1)
                }
                .tag(1)
            
            // Appointments Tab
            AppointmentsView()
                .tabItem {
                    TabItemView(icon: "calendar", title: "Agenda", isSelected: selectedTab == 2)
                }
                .tag(2)
            
            // Profile Tab
            ProfileView()
                .tabItem {
                    TabItemView(icon: "person.circle.fill", title: "Perfil", isSelected: selectedTab == 3)
                }
                .tag(3)
        }
        .tint(.mwPrimary)
        .task {
            await loadDashboardStats()
        }
    }
    
    private func loadDashboardStats() async {
        isLoading = true
        do {
            stats = try await apiService.getDashboardStats()
        } catch {
            print("Error loading dashboard stats: \(error)")
        }
        isLoading = false
    }
}

// MARK: - Tab Item View
struct TabItemView: View {
    let icon: String
    let title: String
    let isSelected: Bool
    
    var body: some View {
        Label(title, systemImage: icon)
    }
}

// MARK: - Home Tab View
struct HomeTabView: View {
    let stats: DashboardStats?
    let isLoading: Bool
    let refreshAction: () async -> Void
    
    @State private var showingCards = false
    @State private var currentDate = Date()
    
    private var greeting: String {
        let hour = Calendar.current.component(.hour, from: currentDate)
        if hour < 12 {
            return "Bom dia!"
        } else if hour < 18 {
            return "Boa tarde!"
        } else {
            return "Boa noite!"
        }
    }
    
    private var dateString: String {
        let formatter = DateFormatter()
        formatter.locale = Locale(identifier: "pt_BR")
        formatter.dateFormat = "EEEE, d 'de' MMMM"
        return formatter.string(from: currentDate).capitalized
    }
    
    var body: some View {
        NavigationView {
            ScrollView(showsIndicators: false) {
                VStack(spacing: MWDesign.Spacing.lg) {
                    // Header Section
                    VStack(spacing: 0) {
                        HStack(alignment: .top) {
                            VStack(alignment: .leading, spacing: MWDesign.Spacing.xxs) {
                                Text(greeting)
                                    .font(.system(size: 28, weight: .bold, design: .rounded))
                                    .foregroundColor(.mwTextPrimary)
                                
                                Text(dateString)
                                    .font(.subheadline)
                                    .foregroundColor(.mwTextSecondary)
                            }
                            
                            Spacer()
                            
                            // Notification Button
                            Button(action: {}) {
                                ZStack(alignment: .topTrailing) {
                                    Circle()
                                        .fill(Color.mwSurfaceSecondary)
                                        .frame(width: 48, height: 48)
                                    
                                    Image(systemName: "bell.fill")
                                        .font(.system(size: 18))
                                        .foregroundColor(.mwTextPrimary)
                                    
                                    // Notification badge
                                    Circle()
                                        .fill(Color.mwError)
                                        .frame(width: 10, height: 10)
                                        .offset(x: 2, y: -2)
                                }
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                        .padding(.top, MWDesign.Spacing.md)
                    }
                    
                    if isLoading {
                        // Loading Skeleton
                        VStack(spacing: MWDesign.Spacing.md) {
                            LazyVGrid(columns: [GridItem(.flexible()), GridItem(.flexible())], spacing: MWDesign.Spacing.md) {
                                ForEach(0..<4) { _ in
                                    SkeletonStatCard()
                                }
                            }
                            
                            ForEach(0..<3) { _ in
                                SkeletonActionCard()
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                    } else if let stats = stats {
                        // Stats Section
                        VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
                            Text("Resumo do Dia")
                                .font(.headline)
                                .foregroundColor(.mwTextSecondary)
                                .padding(.horizontal, MWDesign.Spacing.lg)
                            
                            LazyVGrid(columns: [GridItem(.flexible()), GridItem(.flexible())], spacing: MWDesign.Spacing.md) {
                                ModernStatCard(
                                    title: "Consultas Hoje",
                                    value: "\(stats.todayAppointments)",
                                    icon: "calendar.badge.clock",
                                    gradient: LinearGradient(colors: [Color.mwPrimary, Color.mwPrimaryLight], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.1
                                )
                                
                                ModernStatCard(
                                    title: "Total Pacientes",
                                    value: "\(stats.totalPatients)",
                                    icon: "person.2.fill",
                                    gradient: LinearGradient(colors: [Color.mwSuccess, Color(hex: "34D399")], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.2
                                )
                                
                                ModernStatCard(
                                    title: "Pendentes",
                                    value: "\(stats.pendingAppointments)",
                                    icon: "clock.badge.exclamationmark",
                                    gradient: LinearGradient(colors: [Color.mwWarning, Color(hex: "FBBF24")], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.3
                                )
                                
                                ModernStatCard(
                                    title: "Concluídas",
                                    value: "\(stats.completedToday)",
                                    icon: "checkmark.seal.fill",
                                    gradient: LinearGradient(colors: [Color.mwSecondary, Color(hex: "A78BFA")], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.4
                                )
                            }
                            .padding(.horizontal, MWDesign.Spacing.lg)
                        }
                        
                        // Quick Actions Section
                        VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
                            Text("Ações Rápidas")
                                .font(.headline)
                                .foregroundColor(.mwTextSecondary)
                                .padding(.horizontal, MWDesign.Spacing.lg)
                            
                            VStack(spacing: MWDesign.Spacing.sm) {
                                ModernQuickActionButton(
                                    title: "Novo Paciente",
                                    subtitle: "Cadastrar paciente",
                                    icon: "person.badge.plus",
                                    iconBackground: LinearGradient(colors: [Color.mwPrimary, Color.mwPrimaryLight], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.5
                                )
                                
                                ModernQuickActionButton(
                                    title: "Novo Agendamento",
                                    subtitle: "Agendar consulta",
                                    icon: "calendar.badge.plus",
                                    iconBackground: LinearGradient(colors: [Color.mwSuccess, Color(hex: "34D399")], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.6
                                )
                                
                                ModernQuickActionButton(
                                    title: "Ver Agenda Completa",
                                    subtitle: "Visualizar todos os agendamentos",
                                    icon: "list.bullet.clipboard.fill",
                                    iconBackground: LinearGradient(colors: [Color.mwWarning, Color(hex: "FBBF24")], startPoint: .topLeading, endPoint: .bottomTrailing),
                                    delay: 0.7
                                )
                            }
                            .padding(.horizontal, MWDesign.Spacing.lg)
                        }
                        .padding(.top, MWDesign.Spacing.sm)
                    }
                    
                    Spacer(minLength: 30)
                }
                .padding(.top, MWDesign.Spacing.sm)
            }
            .background(Color.mwBackground)
            .refreshable {
                await refreshAction()
            }
            .navigationBarHidden(true)
        }
        .navigationViewStyle(.stack)
    }
}

// MARK: - Modern Stat Card
struct ModernStatCard: View {
    let title: String
    let value: String
    let icon: String
    let gradient: LinearGradient
    var delay: Double = 0
    
    @State private var appeared = false
    
    var body: some View {
        VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
            HStack {
                ZStack {
                    Circle()
                        .fill(gradient)
                        .frame(width: 42, height: 42)
                    
                    Image(systemName: icon)
                        .font(.system(size: 18, weight: .semibold))
                        .foregroundColor(.white)
                }
                
                Spacer()
            }
            
            VStack(alignment: .leading, spacing: MWDesign.Spacing.xxs) {
                Text(value)
                    .font(.system(size: 28, weight: .bold, design: .rounded))
                    .foregroundColor(.mwTextPrimary)
                
                Text(title)
                    .font(.caption)
                    .fontWeight(.medium)
                    .foregroundColor(.mwTextSecondary)
            }
        }
        .padding(MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.lg)
        .shadow(color: Color.black.opacity(0.04), radius: 8, x: 0, y: 4)
        .overlay(
            RoundedRectangle(cornerRadius: MWDesign.Radius.lg)
                .stroke(Color.mwBorder.opacity(0.5), lineWidth: 1)
        )
        .scaleEffect(appeared ? 1.0 : 0.8)
        .opacity(appeared ? 1.0 : 0)
        .onAppear {
            withAnimation(.spring(response: 0.5, dampingFraction: 0.7).delay(delay)) {
                appeared = true
            }
        }
    }
}

// MARK: - Modern Quick Action Button
struct ModernQuickActionButton: View {
    let title: String
    let subtitle: String
    let icon: String
    let iconBackground: LinearGradient
    var delay: Double = 0
    
    @State private var appeared = false
    @State private var isPressed = false
    
    var body: some View {
        Button(action: {
            // Action to be implemented
        }) {
            HStack(spacing: MWDesign.Spacing.md) {
                ZStack {
                    RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                        .fill(iconBackground)
                        .frame(width: 48, height: 48)
                    
                    Image(systemName: icon)
                        .font(.system(size: 20, weight: .semibold))
                        .foregroundColor(.white)
                }
                
                VStack(alignment: .leading, spacing: 2) {
                    Text(title)
                        .font(.body)
                        .fontWeight(.semibold)
                        .foregroundColor(.mwTextPrimary)
                    
                    Text(subtitle)
                        .font(.caption)
                        .foregroundColor(.mwTextSecondary)
                }
                
                Spacer()
                
                Image(systemName: "chevron.right")
                    .font(.caption)
                    .fontWeight(.semibold)
                    .foregroundColor(.mwTextMuted)
            }
            .padding(MWDesign.Spacing.md)
            .background(Color.mwSurface)
            .cornerRadius(MWDesign.Radius.lg)
            .shadow(color: Color.black.opacity(0.03), radius: 6, x: 0, y: 3)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.lg)
                    .stroke(Color.mwBorder.opacity(0.5), lineWidth: 1)
            )
            .scaleEffect(isPressed ? 0.98 : 1.0)
        }
        .buttonStyle(PlainButtonStyle())
        .simultaneousGesture(
            DragGesture(minimumDistance: 0)
                .onChanged { _ in isPressed = true }
                .onEnded { _ in isPressed = false }
        )
        .offset(x: appeared ? 0 : -30)
        .opacity(appeared ? 1.0 : 0)
        .onAppear {
            withAnimation(.spring(response: 0.5, dampingFraction: 0.8).delay(delay)) {
                appeared = true
            }
        }
    }
}

// MARK: - Skeleton Loading Views
struct SkeletonStatCard: View {
    var body: some View {
        VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
            Circle()
                .fill(Color.mwSurfaceSecondary)
                .frame(width: 42, height: 42)
            
            VStack(alignment: .leading, spacing: MWDesign.Spacing.xs) {
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 60, height: 28)
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 80, height: 14)
            }
        }
        .padding(MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.lg)
        .shimmer()
    }
}

struct SkeletonActionCard: View {
    var body: some View {
        HStack(spacing: MWDesign.Spacing.md) {
            RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                .fill(Color.mwSurfaceSecondary)
                .frame(width: 48, height: 48)
            
            VStack(alignment: .leading, spacing: MWDesign.Spacing.xs) {
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 120, height: 16)
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 160, height: 12)
            }
            
            Spacer()
        }
        .padding(MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.lg)
        .shimmer()
    }
}

// MARK: - Profile View
struct ProfileView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    @State private var showingLogoutAlert = false
    @State private var appeared = false
    
    var body: some View {
        NavigationView {
            ScrollView(showsIndicators: false) {
                VStack(spacing: MWDesign.Spacing.xl) {
                    // Profile Header
                    VStack(spacing: MWDesign.Spacing.lg) {
                        // Avatar
                        ZStack {
                            Circle()
                                .fill(LinearGradient.mwPrimaryGradient)
                                .frame(width: 110, height: 110)
                            
                            if let user = authViewModel.currentUser {
                                Text(String(user.username.prefix(1)).uppercased())
                                    .font(.system(size: 44, weight: .bold, design: .rounded))
                                    .foregroundColor(.white)
                            } else {
                                Image(systemName: "person.fill")
                                    .font(.system(size: 44))
                                    .foregroundColor(.white)
                            }
                        }
                        .shadow(color: Color.mwPrimary.opacity(0.3), radius: 12, x: 0, y: 6)
                        .scaleEffect(appeared ? 1.0 : 0.5)
                        .opacity(appeared ? 1.0 : 0)
                        
                        if let user = authViewModel.currentUser {
                            VStack(spacing: MWDesign.Spacing.xs) {
                                Text(user.username)
                                    .font(.title2)
                                    .fontWeight(.bold)
                                    .foregroundColor(.mwTextPrimary)
                                
                                HStack(spacing: MWDesign.Spacing.xs) {
                                    MWStatusBadge(text: user.role, color: .mwPrimary, style: .subtle)
                                }
                                
                                Text(user.tenantId)
                                    .font(.caption)
                                    .foregroundColor(.mwTextSecondary)
                            }
                            .opacity(appeared ? 1.0 : 0)
                        }
                    }
                    .padding(.top, MWDesign.Spacing.xl)
                    
                    // Settings Options
                    VStack(spacing: MWDesign.Spacing.sm) {
                        ProfileOptionRow(
                            icon: "person.circle.fill",
                            title: "Minha Conta",
                            subtitle: "Informações pessoais",
                            iconColor: .mwPrimary
                        )
                        
                        ProfileOptionRow(
                            icon: "bell.badge.fill",
                            title: "Notificações",
                            subtitle: "Preferências de alerta",
                            iconColor: .mwWarning
                        )
                        
                        ProfileOptionRow(
                            icon: "lock.shield.fill",
                            title: "Privacidade",
                            subtitle: "Segurança e dados",
                            iconColor: .mwSuccess
                        )
                        
                        ProfileOptionRow(
                            icon: "questionmark.circle.fill",
                            title: "Ajuda",
                            subtitle: "Central de suporte",
                            iconColor: .mwAccent
                        )
                        
                        ProfileOptionRow(
                            icon: "info.circle.fill",
                            title: "Sobre",
                            subtitle: "Versão 1.0.0",
                            iconColor: .mwTextMuted
                        )
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    
                    // Logout Button
                    Button(action: {
                        showingLogoutAlert = true
                    }) {
                        HStack(spacing: MWDesign.Spacing.sm) {
                            Image(systemName: "rectangle.portrait.and.arrow.right")
                                .font(.body.weight(.semibold))
                            Text("Sair da Conta")
                                .fontWeight(.semibold)
                        }
                        .foregroundColor(.mwError)
                        .frame(maxWidth: .infinity)
                        .padding(.vertical, MWDesign.Spacing.md)
                        .background(Color.mwError.opacity(0.1))
                        .cornerRadius(MWDesign.Radius.md)
                        .overlay(
                            RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                                .stroke(Color.mwError.opacity(0.2), lineWidth: 1)
                        )
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    .padding(.top, MWDesign.Spacing.lg)
                    
                    Spacer(minLength: 40)
                }
            }
            .background(Color.mwBackground)
            .navigationBarHidden(true)
            .alert("Sair da conta", isPresented: $showingLogoutAlert) {
                Button("Cancelar", role: .cancel) {}
                Button("Sair", role: .destructive) {
                    authViewModel.logout()
                }
            } message: {
                Text("Tem certeza que deseja sair?")
            }
            .onAppear {
                withAnimation(.spring(response: 0.6, dampingFraction: 0.7).delay(0.1)) {
                    appeared = true
                }
            }
        }
        .navigationViewStyle(.stack)
    }
}

// MARK: - Profile Option Row
struct ProfileOptionRow: View {
    let icon: String
    let title: String
    let subtitle: String
    let iconColor: Color
    
    @State private var isPressed = false
    
    var body: some View {
        Button(action: {}) {
            HStack(spacing: MWDesign.Spacing.md) {
                ZStack {
                    RoundedRectangle(cornerRadius: MWDesign.Radius.sm)
                        .fill(iconColor.opacity(0.1))
                        .frame(width: 44, height: 44)
                    
                    Image(systemName: icon)
                        .font(.system(size: 18))
                        .foregroundColor(iconColor)
                }
                
                VStack(alignment: .leading, spacing: 2) {
                    Text(title)
                        .font(.body)
                        .fontWeight(.medium)
                        .foregroundColor(.mwTextPrimary)
                    
                    Text(subtitle)
                        .font(.caption)
                        .foregroundColor(.mwTextSecondary)
                }
                
                Spacer()
                
                Image(systemName: "chevron.right")
                    .font(.caption)
                    .fontWeight(.semibold)
                    .foregroundColor(.mwTextMuted)
            }
            .padding(MWDesign.Spacing.md)
            .background(Color.mwSurface)
            .cornerRadius(MWDesign.Radius.lg)
            .shadow(color: Color.black.opacity(0.02), radius: 4, x: 0, y: 2)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.lg)
                    .stroke(Color.mwBorder.opacity(0.5), lineWidth: 1)
            )
            .scaleEffect(isPressed ? 0.98 : 1.0)
        }
        .buttonStyle(PlainButtonStyle())
        .simultaneousGesture(
            DragGesture(minimumDistance: 0)
                .onChanged { _ in isPressed = true }
                .onEnded { _ in isPressed = false }
        )
    }
}

#Preview {
    DashboardView()
        .environmentObject(AuthViewModel())
}

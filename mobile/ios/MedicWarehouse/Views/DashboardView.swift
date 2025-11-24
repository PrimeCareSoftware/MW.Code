import SwiftUI

struct DashboardView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    @State private var stats: DashboardStats?
    @State private var isLoading = true
    @State private var selectedTab = 0
    
    private let apiService = APIService()
    
    var body: some View {
        NavigationView {
            TabView(selection: $selectedTab) {
                // Home Tab
                HomeTabView(stats: stats, isLoading: isLoading)
                    .tabItem {
                        Label("Início", systemImage: "house.fill")
                    }
                    .tag(0)
                
                // Patients Tab
                PatientsView()
                    .tabItem {
                        Label("Pacientes", systemImage: "person.3.fill")
                    }
                    .tag(1)
                
                // Appointments Tab
                AppointmentsView()
                    .tabItem {
                        Label("Agendamentos", systemImage: "calendar")
                    }
                    .tag(2)
                
                // Profile Tab
                ProfileView()
                    .tabItem {
                        Label("Perfil", systemImage: "person.circle.fill")
                    }
                    .tag(3)
            }
            .accentColor(.blue)
        }
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

struct HomeTabView: View {
    let stats: DashboardStats?
    let isLoading: Bool
    
    var body: some View {
        ScrollView {
            VStack(spacing: 20) {
                // Header
                HStack {
                    VStack(alignment: .leading, spacing: 5) {
                        Text("Bem-vindo!")
                            .font(.title2)
                            .fontWeight(.bold)
                        
                        Text("Painel de Controle")
                            .font(.subheadline)
                            .foregroundColor(.gray)
                    }
                    
                    Spacer()
                    
                    Image(systemName: "bell.badge.fill")
                        .font(.title2)
                        .foregroundColor(.blue)
                }
                .padding()
                
                if isLoading {
                    ProgressView()
                        .scaleEffect(1.5)
                        .padding(50)
                } else if let stats = stats {
                    // Stats Cards
                    LazyVGrid(columns: [GridItem(.flexible()), GridItem(.flexible())], spacing: 15) {
                        StatCard(
                            title: "Consultas Hoje",
                            value: "\(stats.todayAppointments)",
                            icon: "calendar",
                            color: .blue
                        )
                        
                        StatCard(
                            title: "Total Pacientes",
                            value: "\(stats.totalPatients)",
                            icon: "person.3.fill",
                            color: .green
                        )
                        
                        StatCard(
                            title: "Pendentes",
                            value: "\(stats.pendingAppointments)",
                            icon: "clock.fill",
                            color: .orange
                        )
                        
                        StatCard(
                            title: "Concluídas",
                            value: "\(stats.completedToday)",
                            icon: "checkmark.circle.fill",
                            color: .purple
                        )
                    }
                    .padding(.horizontal)
                    
                    // Quick Actions
                    VStack(alignment: .leading, spacing: 15) {
                        Text("Ações Rápidas")
                            .font(.headline)
                            .padding(.horizontal)
                        
                        VStack(spacing: 10) {
                            QuickActionButton(
                                title: "Novo Paciente",
                                icon: "person.badge.plus",
                                color: .blue
                            )
                            
                            QuickActionButton(
                                title: "Novo Agendamento",
                                icon: "calendar.badge.plus",
                                color: .green
                            )
                            
                            QuickActionButton(
                                title: "Ver Agenda",
                                icon: "list.bullet.clipboard",
                                color: .orange
                            )
                        }
                        .padding(.horizontal)
                    }
                    .padding(.top, 20)
                }
                
                Spacer()
            }
        }
        .background(Color(.systemGroupedBackground))
    }
}

struct StatCard: View {
    let title: String
    let value: String
    let icon: String
    let color: Color
    
    var body: some View {
        VStack(spacing: 15) {
            HStack {
                Image(systemName: icon)
                    .font(.title2)
                    .foregroundColor(color)
                
                Spacer()
            }
            
            VStack(alignment: .leading, spacing: 5) {
                Text(value)
                    .font(.title)
                    .fontWeight(.bold)
                
                Text(title)
                    .font(.caption)
                    .foregroundColor(.gray)
            }
            .frame(maxWidth: .infinity, alignment: .leading)
        }
        .padding()
        .background(Color.white)
        .cornerRadius(15)
        .shadow(color: .black.opacity(0.05), radius: 5, x: 0, y: 2)
    }
}

struct QuickActionButton: View {
    let title: String
    let icon: String
    let color: Color
    
    var body: some View {
        Button(action: {
            // Action to be implemented
        }) {
            HStack {
                Image(systemName: icon)
                    .font(.title3)
                    .foregroundColor(color)
                    .frame(width: 40)
                
                Text(title)
                    .fontWeight(.semibold)
                    .foregroundColor(.primary)
                
                Spacer()
                
                Image(systemName: "chevron.right")
                    .foregroundColor(.gray)
            }
            .padding()
            .background(Color.white)
            .cornerRadius(12)
            .shadow(color: .black.opacity(0.05), radius: 3, x: 0, y: 1)
        }
    }
}

struct ProfileView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    
    var body: some View {
        VStack(spacing: 20) {
            // Profile Header
            VStack(spacing: 15) {
                Image(systemName: "person.crop.circle.fill")
                    .font(.system(size: 80))
                    .foregroundColor(.blue)
                
                if let user = authViewModel.currentUser {
                    Text(user.username)
                        .font(.title2)
                        .fontWeight(.bold)
                    
                    Text(user.role)
                        .font(.subheadline)
                        .foregroundColor(.gray)
                    
                    Text("Clínica: \(user.tenantId)")
                        .font(.caption)
                        .foregroundColor(.gray)
                }
            }
            .padding(.top, 40)
            
            Spacer()
            
            // Logout Button
            Button(action: {
                authViewModel.logout()
            }) {
                HStack {
                    Image(systemName: "arrow.right.square")
                    Text("Sair")
                        .fontWeight(.semibold)
                }
                .foregroundColor(.white)
                .frame(maxWidth: .infinity)
                .padding()
                .background(Color.red)
                .cornerRadius(12)
            }
            .padding(.horizontal, 30)
            .padding(.bottom, 40)
        }
        .background(Color(.systemGroupedBackground))
    }
}

#Preview {
    DashboardView()
        .environmentObject(AuthViewModel())
}
